using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Microsoft.Extensions.Hosting;

namespace ZDevTools.XamarinForms
{
    public class XamarinHostLifetime : IHostLifetime, IDisposable
    {
        readonly IHostEnvironment Environment;
        readonly IHostApplicationLifetime Lifetime;
        readonly ILogger Logger;
        readonly XamarinHostLifetimeOptions Options;


        public XamarinHostLifetime(
            IOptions<XamarinHostLifetimeOptions> options,
            IHostEnvironment environment,
            IHostApplicationLifetime applicationLifetime)
            : this(options, environment, applicationLifetime, NullLoggerFactory.Instance) { }

        public XamarinHostLifetime(
            IOptions<XamarinHostLifetimeOptions> options,
            IHostEnvironment environment,
            IHostApplicationLifetime applicationLifetime,
            ILoggerFactory loggerFactory)
        {
            Options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            Lifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            Logger = loggerFactory.CreateLogger("Microsoft.Extensions.Hosting.Host");
        }

        CancellationTokenRegistration _applicationStartedRegistration;
        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            if (!Options.SuppressStatusMessages)
            {
                _applicationStartedRegistration = Lifetime.ApplicationStarted.Register(state =>
                {
                    ((XamarinHostLifetime)state).onApplicationStarted();
                },
                this);
            }

            return Task.CompletedTask;
        }

        private void onApplicationStarted()
        {
            Logger.LogInformation("Application started.");
            Logger.LogInformation("Hosting environment: {envName}", Environment.EnvironmentName);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _applicationStartedRegistration.Dispose();
        }
    }
}
