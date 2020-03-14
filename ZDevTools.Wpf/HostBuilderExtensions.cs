using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Splat;
using System;
using System.Windows;
using Splat.Microsoft.Extensions.DependencyInjection;
using ZDevTools.Wpf.ReactiveUI;

namespace ZDevTools.Wpf
{
    /// <summary>
    /// This contains the rReactiveUi extensions for Microsoft.Extensions.Hosting 
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Configure a ReactiveUI application
        /// </summary>
        /// <param name="hostBuilder">IHostBuilder</param>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder ConfigureReactiveUi(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) =>
            {
                serviceCollection.UseMicrosoftDependencyResolver();
                Locator.CurrentMutable.Initialize();
                //��Ҫʹ�����·�ʽ���⽫����Splat�����ϴ��뵼��Splat�ڲ�����һ���µ�ServiceProvider�� �� Microsoft.Extensions.Logging ���õ� ILoggerFactory������ͬһ�����󣬴Ӷ�������־��¼˳����ң����δ֪���⣬��õķ�ʽ���ǲ�����Splat�ڽ�����־ϵͳ��
                //�� Splat �� Microsoft.Extensions.Logging д����־
                //Locator.CurrentMutable.UseMicrosoftExtensionsLoggingWithWrappingFullLogger(Locator.Current.GetService<ILoggerFactory>());
            })
            // ����Ҫ����־д��Splat����Ϊ�û���ʹ���Լ�����־ϵͳ
            //.ConfigureLogging(loggingBuilder =>
            //{
            //    //�� Microsoft.Extensions.Logging �� Splat д����־
            //    loggingBuilder.AddSplat();
            //})
            ;
        }

        public static IHostBuilder ConfigureShell<TShell>(this IHostBuilder hostBuilder)
            where TShell : Window
        {
            return hostBuilder.ConfigureServices(serviceCollection =>
            {
                serviceCollection.AddHostedService<WpfHostedService<TShell>>();
            });
        }

        public static IHostBuilder ConfigureShell<TShell>(this IHostBuilder hostBuilder, Action<StartupOptions> configOptions)
            where TShell : Window
        {
            return hostBuilder.ConfigureServices(serviceCollection =>
            {
                serviceCollection.Configure(configOptions);
                serviceCollection.AddHostedService<WpfHostedService<TShell>>();
            });
        }

        public static IHostBuilder ConfigureApplication<TApplication>(this IHostBuilder hostBuilder)
            where TApplication : Application
        {
            return hostBuilder.ConfigureServices(serviceCollection =>
            {
                serviceCollection.AddSingleton<Application, TApplication>();
            });
        }
    }
}