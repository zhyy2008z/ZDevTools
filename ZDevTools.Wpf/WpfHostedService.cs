using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ZDevTools.Wpf
{
    /// <summary>
    /// This hosts a WPF service, making sure the lifecycle is managed
    /// </summary>
    class WpfHostedService<TShell> : IHostedService
        where TShell : Window
    {
        readonly ILogger<WpfHostedService<TShell>> Logger;
        readonly IServiceProvider ServiceProvider;
        readonly IOptions<StartupOptions> Options;

        /// <summary>
        /// The constructor which takes all the DI objects
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="serviceProvider">IServiceProvider</param>
        /// <param name="wpfContext">IWpfContext</param>
        public WpfHostedService(ILogger<WpfHostedService<TShell>> logger, IServiceProvider serviceProvider, IOptions<StartupOptions> options)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
            Options = options;
        }

        public bool IsRunning { get; private set; }


        Dispatcher _dispatcher;
        Application _application;
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Thread thread = new Thread(state =>
             {
                 // Create our SynchronizationContext, and install it:
                 SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(_dispatcher = Dispatcher.CurrentDispatcher));

                 if (Options.Value.IsUseSplashScreen)
                 {
                     SplashScreen splashScreen = new SplashScreen(Options.Value.SplashScreenResourceName);
                     splashScreen.Show(autoClose: true);
                 }

                 // Create the new WPF application
                 _application = ServiceProvider.GetService<Application>() ?? new Application();
                 // Shutdown Mode OnMainWindowClose
                 _application.ShutdownMode = ShutdownMode.OnMainWindowClose;
                 // Register to the WPF application exit to stop the host application
                 _application.Exit += (s, e) =>
                  {
                      IsRunning = false;

                      ServiceProvider.GetService<IHostApplicationLifetime>().StopApplication();
                  };

                 IsRunning = true;

                 // Run the WPF application in this thread which was specifically created for it, with the specified shell
                 if (ServiceProvider.GetRequiredService<TShell>() is Window wpfShell)
                 {
                     _application.Run(wpfShell);
                 }
                 else
                 {
                     throw new ApplicationException("未能找到Shell。");
                 }
             })
            { IsBackground = true };

            thread.SetApartmentState(ApartmentState.STA);
            // Start the new WPF thread
            thread.Start();

            // Make the UI thread go
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var isConsoleLifetime = (ServiceProvider.GetService<IHostLifetime>() is ConsoleLifetime);
            if (IsRunning && !isConsoleLifetime) //由于ConsoleLifetime控制台会导致后面一句出错，因此不让其调用了。
            {
                Logger.LogDebug("Stopping WPF due to application exit.");
                // Stop application
                await _dispatcher.InvokeAsync(() => _application.Shutdown());
            }
        }
    }
}
