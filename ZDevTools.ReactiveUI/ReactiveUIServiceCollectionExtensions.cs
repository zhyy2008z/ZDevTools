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

                //应将IScreen优先处理，因为这个接口更为重要
                if (typeof(IScreen).IsAssignableFrom(type))  //Screen(As Scope)，允许一个应用中IScreen出现多次（各Scope内仅实例化一次）
                {
                    serviceCollection.AddScoped(type);
                    serviceCollection.AddScoped(typeof(IScreen), serviceProvider => serviceProvider.GetService(type));
                }
                else if (typeof(ReactiveObject).IsAssignableFrom(type))//ViewModel
                    serviceCollection.AddTransient(type);
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
