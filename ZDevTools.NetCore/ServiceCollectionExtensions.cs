using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;


namespace ZDevTools.NetCore
{
    /// <summary>
    /// 可以提供的所有方法
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加加密提供器
        /// </summary>
        public static IServiceCollection AddEncdecProvider(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<IEncdecProvider, Services.EncdecProvider>();
            serviceCollection.Configure<EncdecOptions>(configuration);
            return serviceCollection;
        }
    }
}
