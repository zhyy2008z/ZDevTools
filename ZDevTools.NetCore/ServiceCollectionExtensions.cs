using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;


namespace ZDevTools.NetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddZDevTools(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IEncdecProvider, Services.EncdecProvider>();
            return serviceCollection;
        }
    }
}
