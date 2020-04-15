using ReactiveUI;
using Splat;

namespace ZDevTools.WindowsForms.ReactiveUI
{
    public static class DependencyResolverExtensions
    {

        public static void Initialize(this IDependencyResolver resolver)
        {
            // These Initialize methods will add ReactiveUI platform registrations to your container
            // They MUST be present if you override the default Locator
            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.RegisterConstant<IBindingTypeConverter>(new NumberBindingTypeConverter());
            resolver.RegisterConstant<IBindingTypeConverter>(new EnumBindingTypeConverter());
        }

    }
}
