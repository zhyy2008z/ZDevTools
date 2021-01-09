using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ZDevTools.NetCore;
using ZDevTools.NetCore.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 可以提供的所有方法
    /// </summary>
    public static class NetCoreServiceCollectionExtensions
    {
        /// <summary>
        /// 添加加密提供器
        /// </summary>
        public static IServiceCollection AddEncdecProvider(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<IEncdecProvider, EncdecProvider>();
            serviceCollection.Configure<EncdecOptions>(configuration);
            return serviceCollection;
        }

        /// <summary>
        /// 添加加密提供器
        /// </summary>
        public static IServiceCollection AddEncdecProvider(this IServiceCollection serviceCollection, Action<EncdecOptions> configureOptions)
        {
            serviceCollection.AddSingleton<IEncdecProvider, EncdecProvider>();
            serviceCollection.Configure(configureOptions);
            return serviceCollection;
        }
    }
}
