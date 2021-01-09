using Microsoft.Extensions.DependencyInjection;

using System;
using System.Windows;

using ZDevTools.Wpf;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// This contains the rReactiveUi extensions for Microsoft.Extensions.Hosting 
    /// </summary>
    public static class WpfHostBuilderExtensions
    {
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