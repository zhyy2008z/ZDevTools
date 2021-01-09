using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// This contains the rReactiveUi extensions for Microsoft.Extensions.Hosting 
    /// </summary>
    public static class ReactiveUIHostBuilderExtensions
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
                //不要使用以下方式，这将导致Splat（以上代码导致Splat内部创建一个新的ServiceProvider） 与 Microsoft.Extensions.Logging 所用的 ILoggerFactory，不是同一个对象，从而导致日志记录顺序混乱，造成未知问题，最好的方式就是不是用Splat内建的日志系统。
                //从 Splat 向 Microsoft.Extensions.Logging 写入日志
                //Locator.CurrentMutable.UseMicrosoftExtensionsLoggingWithWrappingFullLogger(Locator.Current.GetService<ILoggerFactory>());
            })
            // 不需要将日志写回Splat，因为用户会使用自己的日志系统
            //.ConfigureLogging(loggingBuilder =>
            //{
            //    //从 Microsoft.Extensions.Logging 向 Splat 写入日志
            //    loggingBuilder.AddSplat();
            //})
            ;
        }
    }
}