using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using Splat;

namespace ZDevTools.Wpf.ReactiveUI
{
    public static class DependencyResolverExtensions
    {
        public static void Initialize(this IMutableDependencyResolver dependencyResolver)
        {
            dependencyResolver.InitializeSplat();
            dependencyResolver.InitializeReactiveUI();
            dependencyResolver.RegisterConstant<IBindingTypeConverter>(new NumberBindingTypeConverter());
            dependencyResolver.RegisterConstant<IBindingTypeConverter>(new EnumBindingTypeConverter());
        }
    }
}
