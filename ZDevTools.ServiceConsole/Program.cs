using Microsoft.Extensions.Hosting;
using ReactiveUI;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using Serilog;
using Serilog.Events;
using Splat.Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using System.Linq;
using System.Runtime.Loader;
using Microsoft.Extensions.Options;
using ZDevTools.Wpf;
using ZDevTools.ServiceCore;

namespace ZDevTools.ServiceConsole
{
    class Program
    {
        static EventSink _eventSink;

        static async Task Main(string[] args)
        {
            //设置系统默认区域文化及日期格式化方式
            var culture = CultureInfo.CreateSpecificCulture("zh-CN");
            culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            culture.DateTimeFormat.LongTimePattern = "HH:mm:ss";
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            _eventSink = new EventSink();
            Log.Logger = configLogger(new LoggerConfiguration(), out var env, out var formalEnv)
                           .Enrich.FromLogContext()
                           .WriteTo.Console()
                           .WriteTo.Sink(_eventSink)
                           .CreateLogger();

            Log.Verbose($"DOTNET_ENVIRONMENT:{env}");
            Log.Verbose($"Formal Environment:{formalEnv}");
            try
            {
                Log.Verbose("start run");
                var host = CreateHostBuilder(args).Build();
                host.Services.UseMicrosoftDependencyResolver();
#if DEBUG
                //这句话是有作用的，只是有时候Automapper也会提前触发一些验证，感觉好像这句没用似的。
                host.Services.GetRequiredService<IMapper>().ConfigurationProvider.AssertConfigurationIsValid();
#endif

                if (args.Length == 2 && args[0] == "-Daemon") //作为Windows服务运行
                {
                    var services = host.Services.GetRequiredService<MainViewModel>().GetServices();

                    if ((from s in services
                         where s is WindowsServiceBase && s.ServiceName == args[1]
                         select s).SingleOrDefault() is WindowsServiceBase service)
                        System.ServiceProcess.ServiceBase.Run(service);
                }
                else
                {
                    //正常启动软件界面
#if !DEBUG
                    //处理所有异常
                    AppDomain.CurrentDomain.UnhandledException += currentDomain_UnhandledException;
#endif

                    await host.RunAsync();
                }

            }
            catch (Exception exception)
            {
                Log.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                //nsure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureReactiveUi()
            .ConfigureApplication<App>()
            .ConfigureShell<MainWindow>()
            .ConfigureServices(serviceCollection =>
            {
                serviceCollection.AddReactiveUIComponents(typeof(Program).Assembly);
                serviceCollection.AddAutoMapper(config =>
                {
                    config.AddCollectionMappers();
                }, typeof(Program));
                serviceCollection.AddSingleton(_eventSink);
            })
            .ConfigureServices(configureAppServices)
            .UseSerilog();

        static LoggerConfiguration configLogger(LoggerConfiguration config, out string env, out string formalEnv)
        {
            env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            if (string.IsNullOrEmpty(env)) env = Environments.Production;

            config.MinimumLevel.Verbose();

            if (env.Equals(Environments.Production, StringComparison.InvariantCultureIgnoreCase))
            {
                formalEnv = Environments.Production;
                return config.MinimumLevel.Override("Microsoft", LogEventLevel.Error);
            }
            else if (env.Equals(Environments.Staging, StringComparison.InvariantCultureIgnoreCase))
            {
                formalEnv = Environments.Staging;
                return config.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
            }
            else
            {
                formalEnv = Environments.Development;
                return config.MinimumLevel.Override("Microsoft", LogEventLevel.Information);
            }
        }

        private static void configureAppServices(HostBuilderContext hostBuilderContext, IServiceCollection serviceCollection)
        {
            //服务公共配置
            serviceCollection.Configure<ServiceOptions>(hostBuilderContext.Configuration.GetSection(nameof(ServiceOptions)));
            serviceCollection.Configure<ConsoleOptions>(hostBuilderContext.Configuration.GetSection(nameof(ConsoleOptions)));

            //配置单例服务RedisManager
            serviceCollection.AddSingleton(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<ServiceOptions>>();
                return string.IsNullOrEmpty(options.Value.RedisServer) ? null : new ServiceStack.Redis.RedisManagerPool(options.Value.RedisServer);
            });

            serviceCollection.AddSingleton<IDialogs, Services.Dialogs>();
            serviceCollection.AddSingleton<IUtility, Services.Utility>();

            //加载模块
            var moduleType = typeof(IServiceModule);

            foreach (var fileInfo in hostBuilderContext.HostingEnvironment.ContentRootFileProvider.GetDirectoryContents(string.Empty).Where(fi => fi.Name.EndsWith("ServiceModule.dll", StringComparison.OrdinalIgnoreCase)))
                foreach (var type in AssemblyLoadContext.Default.LoadFromAssemblyPath(fileInfo.PhysicalPath).GetTypes().Where(type => moduleType.IsAssignableFrom(type) && !type.IsAbstract))
                    ((IServiceModule)Activator.CreateInstance(type)).ConfigureServices(hostBuilderContext, serviceCollection);
        }

        private static void currentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            handleException(e.ExceptionObject); //未预料到的任何错误，记录日志即可，程序会因异常未处理而退出
        }

        private static void handleException(object exception)
        {
            if (exception == null)
                Log.Fatal("！v！<发生异常，但异常对象为空>！v！");
            else
            {
                var except = exception as Exception;
                if (except == null)
                    Log.Fatal($"！v！<未知类型的异常:{exception}>！v！");
                else
                    Log.Fatal("！v！<未捕获的异常>！v！", except);
            }
        }
    }
}
