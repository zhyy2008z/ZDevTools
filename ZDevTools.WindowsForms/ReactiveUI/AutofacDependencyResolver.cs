using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Splat;

namespace ZDevTools.WindowsForms.ReactiveUI
{
   public class AutofacDependencyResolver : IDependencyResolver
    {
        readonly HashSet<(Type, string)> Registry = new HashSet<(Type, string)>();
        private static (Type, string) getKey(Type serviceType, string contract = null)
        {
            return (serviceType, contract ?? string.Empty);
        }

        IContainer _container;
        public IContainer BuildContainer()
        {
            return _container = ContainerBuilder.Build();
        }

        public ILifetimeScope BeginScope()
        {
            return _scope = _container.BeginLifetimeScope();
        }

        private ILifetimeScope _scope;
        private readonly ContainerBuilder ContainerBuilder;

        public AutofacDependencyResolver(ContainerBuilder builder)
        {
            ContainerBuilder = builder;
        }

        public bool HasRegistration(Type serviceType)
        {
            return Registry.Contains(getKey(serviceType));
        }

        public bool HasRegistration(Type serviceType, string contract = null)
        {
            return Registry.Contains(getKey(serviceType, contract));
        }

        public object GetService(Type serviceType, string contract = null)
        {
            try
            {
                return string.IsNullOrEmpty(contract)
                    ? _scope.Resolve(serviceType)
                    : _scope.ResolveNamed(contract, serviceType);
            }
            catch (DependencyResolutionException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType, string contract = null)
        {
            try
            {
                var enumerableType = typeof(IEnumerable<>).MakeGenericType(serviceType);
                object instance = string.IsNullOrEmpty(contract)
                    ? _scope.Resolve(enumerableType)
                    : _scope.ResolveNamed(contract, enumerableType);
                return ((IEnumerable)instance).Cast<object>();
            }
            catch (DependencyResolutionException)
            {
                return null;
            }
        }



        public void Register(Func<object> factory, Type serviceType, string contract = null)
        {
            Registry.Add(getKey(serviceType, contract));
            if (string.IsNullOrEmpty(contract))
            {
                ContainerBuilder.Register(x => factory()).As(serviceType).AsImplementedInterfaces();
            }
            else
            {
                ContainerBuilder.Register(x => factory()).Named(contract, serviceType).AsImplementedInterfaces();
            }
        }

        public void UnregisterCurrent(Type serviceType, string contract = null)
        {
            throw new NotImplementedException();
        }

        public void UnregisterAll(Type serviceType, string contract = null)
        {
            throw new NotImplementedException();
        }

        public IDisposable ServiceRegistrationCallback(Type serviceType, string contract, Action<IDisposable> callback)
        {
            // this method is not used by RxUI
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of the instance.
        /// </summary>
        /// <param name="disposing">Whether or not the instance is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _scope.ComponentRegistry?.Dispose();
                _container.Dispose();
            }
        }
    }
}
