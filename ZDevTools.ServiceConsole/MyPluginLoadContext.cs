using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceConsole
{
    class MyPluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public MyPluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var assembly = Default.Assemblies.FirstOrDefault(a => a.FullName == assemblyName.FullName);
            if (assembly != null) return assembly;

            // This will fallback to loading the assembly from default context.
            //if (AssemblyLoadContext.Default.Assemblies.Any(a => a.FullName == assemblyName.FullName))
            //    return null;

            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);

            if (assemblyPath != null)
                return LoadFromAssemblyPath(assemblyPath);

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }
            return IntPtr.Zero;
        }
    }

}
