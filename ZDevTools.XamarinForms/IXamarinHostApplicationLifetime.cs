using Microsoft.Extensions.Hosting;

namespace ZDevTools.XamarinForms
{
    /// <summary>
    /// Allows consumers to perform cleanup during application sleep/resume.
    /// </summary>
    public interface IXamarinHostApplicationLifetime : IHostApplicationLifetime
    {
        /// <summary>
        /// Triggered when the application has gone to sleep.
        /// </summary>
        ILifecycleRegister ApplicationSleeping { get; }

        /// <summary>
        /// Triggered when the application has resumed.
        /// </summary>
        ILifecycleRegister ApplicationResuming { get; }
    }
}