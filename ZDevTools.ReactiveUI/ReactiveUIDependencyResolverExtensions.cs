using System;
using System.Collections.Generic;
using System.Text;

using ReactiveUI.Converters;
using ReactiveUI;

namespace Splat
{
    public static class ReactiveUIDependencyResolverExtensions
    {
        public static void Initialize(this IMutableDependencyResolver dependencyResolver)
        {
            dependencyResolver.InitializeSplat();
            dependencyResolver.InitializeReactiveUI();
            dependencyResolver.RegisterConstant<IBindingTypeConverter>(new NumberBindingTypeConverter());
            dependencyResolver.RegisterConstant<IBindingTypeConverter>(new EnumBindingTypeConverter());
            dependencyResolver.RegisterConstant(MessageBus.Current);//将MessageBus注册进容器
        }
    }
}
