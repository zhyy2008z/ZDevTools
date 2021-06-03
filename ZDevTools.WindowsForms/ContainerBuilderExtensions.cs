using System.Linq;
using System.Reflection;
using Autofac;
using ReactiveUI;
using ZDevTools.WindowsForms.Services;
using Splat;
using ZDevTools.WindowsForms.ReactiveUI;

namespace ZDevTools.WindowsForms
{

    /// <summary>
    /// ContainerBuilder扩展函数
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// 注册本类库提供的组件
        /// </summary>
        public static ContainerBuilder AddWindowsForms(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<UITaskManager>().As<IUITaskManager>();

            return containerBuilder;
        }

        /// <summary>
        /// 注册ReactiveUI所需组件
        /// </summary>
        public static ContainerBuilder RegisterReactiveUIComponents(this ContainerBuilder containerBuilder, Assembly assembly)
        {
            //注册所有的View和ViewModel
            foreach (var type in assembly.DefinedTypes)
            {
                if (type.IsAbstract) continue;

                if (type.IsAssignableTo<ReactiveObject>())//ViewModel
                    containerBuilder.RegisterType(type);
                else if (type.IsAssignableTo<IScreen>())  //Screen(Per Scope)，允许一个应用中IScreen出现多次（各Scope内仅实例化一次）
                    containerBuilder.RegisterType(type).AsSelf().As<IScreen>().InstancePerLifetimeScope();
                else //Maybe View
                {
                    var type2 = type.ImplementedInterfaces.FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IViewFor<>));
                    if (type2 != null)
                        containerBuilder.RegisterType(type).AsSelf().As(type2);
                }
            }

            return containerBuilder;
        }

        /// <summary>
        /// 为Splat使用Autofac依赖解析器
        /// </summary>
        /// <param name="containerBuilder"></param>
        /// <returns></returns>
        public static AutofacDependencyResolver UseAutofacDependencyResolver(this ContainerBuilder containerBuilder)
        {
            var resolver = new AutofacDependencyResolver(containerBuilder);
            resolver.Initialize();
            Locator.SetLocator(resolver);
            return resolver;
        }
    }
}
