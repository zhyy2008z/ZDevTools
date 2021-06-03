using System.Linq;
using System.Reflection;

using ReactiveUI;


namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// ReactiveUI服务扩展
    /// </summary>
    public static class ReactiveUIServiceCollectionExtensions
    {
        /// <summary>
        /// 添加ReactiveUI组件
        /// </summary>
        public static IServiceCollection AddReactiveUIComponents(this IServiceCollection serviceCollection, Assembly assembly)
        {
            //注册所有的View和ViewModel
            foreach (var type in assembly.DefinedTypes)
            {
                if (type.IsAbstract) continue;

                if (typeof(ReactiveObject).IsAssignableFrom(type))//ViewModel
                    serviceCollection.AddTransient(type);
                else if (typeof(IScreen).IsAssignableFrom(type))  //Screen(As Scope)
                {
                    var realType = type.AsType();
                    serviceCollection.AddScoped(realType);
                    serviceCollection.AddScoped(typeof(IScreen), serviceProvider => serviceProvider.GetService(realType));
                }
                else //Maybe View
                {
                    var type2 = type.ImplementedInterfaces.FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IViewFor<>));
                    if (type2 != null)
                    {
                        serviceCollection.AddTransient(type);
                        serviceCollection.AddTransient(type2, type);
                    }
                }
            }

            return serviceCollection;
        }

    }
}
