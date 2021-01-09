using System.Linq;
using System.Reflection;

using ReactiveUI;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class ReactiveUIServiceCollectionExtensions
    {
        public static IServiceCollection AddReactiveUIComponents(this IServiceCollection serviceCollection, Assembly assembly)
        {
            //注册所有的View和ViewModel
            foreach (var type in assembly.DefinedTypes)
            {
                if (type.IsAbstract) continue;

                if (typeof(IScreen).IsAssignableFrom(type))  //Screen(Singleton)
                {
                    var realType = type.AsType();
                    serviceCollection.AddSingleton(realType);
                    serviceCollection.AddSingleton(typeof(IScreen), serviceProvider => serviceProvider.GetService(realType));
                }
                else if (typeof(ReactiveObject).IsAssignableFrom(type))//ViewModel
                    serviceCollection.AddTransient(type);
                else //View
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
