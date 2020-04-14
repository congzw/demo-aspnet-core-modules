using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;
using MyApp.Common.Modules.Impl;

namespace MyApp.Common.Modules
{
    public static class ModuleAssemblyHelper
    {
        static ModuleAssemblyHelper()
        {
            GetAssemblies = () => GetModuleAssemblies(null);
        }

        public static Func<IEnumerable<Assembly>> GetAssemblies { get; set; }
        
        private static IEnumerable<Assembly> GetModuleAssemblies(string modulePrefix = null)
        {
            if (string.IsNullOrWhiteSpace(modulePrefix))
            {
                modulePrefix = TryGetPrefix();
            }

            if (string.IsNullOrWhiteSpace(modulePrefix))
            {
                throw new ArgumentNullException(nameof(modulePrefix));
            }

            var allLib = DependencyContext.Default.CompileLibraries;
            //var allLib = DependencyContext.Default.RuntimeLibraries;
            var libs = allLib.Where(x => x.Name.StartsWith(modulePrefix, StringComparison.OrdinalIgnoreCase));
            var assemblies = libs.Select(lib =>
                {
                    try
                    {
                        return AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            ).Where(x => x != null).ToList();

            //todo figure out why?
            //var demoDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyApp.Module.Demos.dll");
            //var demoDll = Assembly.LoadFile(demoDllPath);
            //assemblies.Add(demoDll);

            return assemblies;
        }
        private static string TryGetPrefix()
        {
            var ns = typeof(ModuleServiceContext).Namespace;
            if (ns != null)
            {
                var modulePrefix = ns.Split(".").FirstOrDefault();
                return modulePrefix;
            }
            //todo: read from config?
            return "";
        }
    }
}