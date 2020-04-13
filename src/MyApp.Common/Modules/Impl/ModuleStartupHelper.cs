using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace MyApp.Common.Modules.Impl
{
    internal class ModuleStartupHelper
    {
        public static void AddAllModuleStartup(IServiceCollection services, IEnumerable<Assembly> moduleAssemblies)
        {
            if (moduleAssemblies == null)
            {
                throw new ArgumentNullException(nameof(moduleAssemblies));
            }

            var startupInterfaceType = typeof(IModuleStartup);
            var startupTypes = moduleAssemblies.SelectMany(x => x.ExportedTypes.Where(t => startupInterfaceType.IsAssignableFrom(t)))
                .Where(t => !t.IsAbstract && !t.IsInterface).ToList();

            foreach (var startupType in startupTypes)
            {
                services.AddSingleton(startupType);
                services.AddSingleton(startupInterfaceType, sp => sp.GetService(startupType));
            }
        }

        public static IEnumerable<Assembly> GetModuleAssemblies(string modulePrefix = null)
        {
            if (string.IsNullOrWhiteSpace(modulePrefix))
            {
                var ns = typeof(ModuleStartupHelper).Namespace;
                if (ns != null)
                {
                    modulePrefix = ns.Split(".").FirstOrDefault();
                }
            }

            if (string.IsNullOrWhiteSpace(modulePrefix))
            {
                throw new ArgumentNullException(nameof(modulePrefix));
            }


            var allLib = DependencyContext.Default.CompileLibraries;
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
            ).Where(x => x != null);
            return assemblies;
        }
    }
}
