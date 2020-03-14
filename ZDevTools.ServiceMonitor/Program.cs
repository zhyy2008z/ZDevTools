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

namespace ZDevTools.ServiceMonitor
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

#if !DEBUG
                //处理所有异常
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif

                await host.RunAsync();

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

            if (env.Equals(Environments.Production, StringComparison.InvariantCultureIgnoreCase))
            {
                formalEnv = Environments.Production;
                return config.MinimumLevel.Information()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Error);
            }
            else if (env.Equals(Environments.Staging, StringComparison.InvariantCultureIgnoreCase))
            {
                formalEnv = Environments.Staging;
                return config.MinimumLevel.Verbose()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
            }
            else
            {
                formalEnv = Environments.Development;
                return config.MinimumLevel.Verbose()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Information);
            }
        }

        private static void configureAppServices(HostBuilderContext hostBuilderContext, IServiceCollection serviceCollection)
        {
            serviceCollection.Configure<MonitorOptions>(hostBuilderContext.Configuration.GetSection(nameof(MonitorOptions)));

            serviceCollection.AddSingleton(serviceProvider => new ServiceStack.Redis.RedisManagerPool(serviceProvider.GetRequiredService<IOptions<MonitorOptions>>().Value.RedisServer));



        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
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
