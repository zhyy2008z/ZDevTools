using Microsoft.Extensions.DependencyInjection;

using ReactiveUI;

using System.Linq;
using System.Reflection;


namespace ZDevTools.Wpf
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddReactiveUIComponents(this IServiceCollection serviceCollection, Assembly assembly)
        {
            //注册所有的View和ViewModel
            foreach (var type in assembly.DefinedTypes)
            {
                if (type.IsAbstract) continue;

                if (typeof(IScreen).IsAssignableFrom(type))  //Screen
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
