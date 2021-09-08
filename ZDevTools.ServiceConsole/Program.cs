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
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;

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

            if (!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");

            Serilog.Debugging.SelfLog.Enable(TextWriter.Synchronized(File.CreateText("logs\\serilog_self.log")));

            _eventSink = new EventSink();
            Log.Logger = configLogger(new LoggerConfiguration(), out var env, out var formalEnv)
                           .Enrich.FromLogContext()
                           .WriteTo.Console()
                           .WriteTo.Sink(_eventSink)
                           .WriteTo.File("logs\\log.log", LogEventLevel.Information, rollingInterval: RollingInterval.Day)
                           .CreateLogger();

            Log.Verbose($"DOTNET_ENVIRONMENT:{env}");
            Log.Verbose($"Formal Environment:{formalEnv}");
            try
            {
                Log.Verbose("start run");
                var host = CreateHostBuilder(args).Build();
                host.Services.UseMicrosoftDependencyResolver();

                if (Debugger.IsAttached) //这句话是有作用的，只是有时候Automapper也会提前触发一些验证，感觉好像这句没用似的。
                    host.Services.GetRequiredService<IMapper>().ConfigurationProvider.AssertConfigurationIsValid();

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

                    if (!Debugger.IsAttached)  //处理所有异常
                        AppDomain.CurrentDomain.UnhandledException += currentDomain_UnhandledException;

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
            .ConfigureAppConfiguration((HostBuilderContext hostBuilderContext, IConfigurationBuilder config) =>
            {
                IHostEnvironment hostingEnvironment = hostBuilderContext.HostingEnvironment;
                bool value = hostBuilderContext.Configuration.GetValue("hostBuilder:reloadConfigOnChange", defaultValue: true);

                string modulePath = hostBuilderContext.Configuration["ModulePath"];
                if (string.IsNullOrEmpty(modulePath))
                    foreach (var fileInfo in hostBuilderContext.HostingEnvironment.ContentRootFileProvider.GetDirectoryContents("plugins").Where(fi => fi.IsDirectory))
                    {
                        var pluginDllPath = Path.Combine(fileInfo.PhysicalPath, fileInfo.Name + ".dll");
                        if (!File.Exists(pluginDllPath)) continue; //跳过插件文件名与插件文件夹名不一致的插件
                        config.AddJsonFile(Path.Combine(fileInfo.PhysicalPath, "appsettings.json"), optional: true, value)
                        .AddJsonFile(Path.Combine(fileInfo.PhysicalPath, "appsettings." + hostingEnvironment.EnvironmentName + ".json"), optional: true, value);
                    }
                else
                {
                    var moduleFolder = Path.GetDirectoryName(modulePath);
                    config.AddJsonFile(Path.Combine(moduleFolder, "appsettings.json"), optional: true, value)
                    .AddJsonFile(Path.Combine(moduleFolder, "appsettings." + hostingEnvironment.EnvironmentName + ".json"), optional: true, value);
                }
            })
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

            var minimumLevel = config.MinimumLevel;
            minimumLevel.Verbose();

            if (env.Equals(Environments.Production, StringComparison.InvariantCultureIgnoreCase))
            {
                formalEnv = Environments.Production;
                minimumLevel.Override("Microsoft", LogEventLevel.Error);
                minimumLevel.Override("System", LogEventLevel.Error);
            }
            else if (env.Equals(Environments.Staging, StringComparison.InvariantCultureIgnoreCase))
            {
                formalEnv = Environments.Staging;
                minimumLevel.Override("Microsoft", LogEventLevel.Warning);
                minimumLevel.Override("System", LogEventLevel.Warning);
            }
            else
            {
                formalEnv = Environments.Development;
                minimumLevel.Override("Microsoft", LogEventLevel.Information);
                minimumLevel.Override("System", LogEventLevel.Information);
            }

            return config;
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
            string modulePath = hostBuilderContext.Configuration["ModulePath"];
            if (string.IsNullOrEmpty(modulePath))
                foreach (var fileInfo in hostBuilderContext.HostingEnvironment.ContentRootFileProvider.GetDirectoryContents("plugins").Where(fi => fi.IsDirectory))
                {
                    var pluginDllPath = Path.Combine(fileInfo.PhysicalPath, fileInfo.Name + ".dll");
                    if (!File.Exists(pluginDllPath)) continue; //跳过插件文件名与插件文件夹名不一致的插件
                    var context = new MyPluginLoadContext(pluginDllPath);
                    var assembly = context.LoadFromAssemblyName(new AssemblyName(fileInfo.Name));
                    foreach (var type in assembly.GetTypes().Where(type => moduleType.IsAssignableFrom(type) && !type.IsAbstract))
                        ((IServiceModule)Activator.CreateInstance(type)).ConfigureServices(hostBuilderContext, serviceCollection);
                }
            else
            {
                var context = new MyPluginLoadContext(modulePath);
                var assembly = context.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(modulePath)));
                foreach (var type in assembly.GetTypes().Where(type => moduleType.IsAssignableFrom(type) && !type.IsAbstract))
                    ((IServiceModule)Activator.CreateInstance(type)).ConfigureServices(hostBuilderContext, serviceCollection);
            }
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
                    Log.Fatal(except, "！v！<未捕获的异常>！v！");
            }
        }
    }
}
