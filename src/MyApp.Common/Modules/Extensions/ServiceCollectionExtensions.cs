using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Common.Modules.Impl;

namespace MyApp.Common.Modules.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IModuleServiceContext AddMyModules(this IServiceCollection services, Action<IModuleServiceContext> configure = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            var contextInterfaceType = typeof(IModuleServiceContext);
            var context = services.LastOrDefault(d => d.ServiceType == contextInterfaceType)?.ImplementationInstance as IModuleServiceContext;
            if (context == null)
            {
                context = new ModuleServiceContext()
                {
                    ApplicationServices = services
                };

                var contextDefaultImplType = context.GetType();
                services.AddSingleton(contextDefaultImplType, context);
                services.AddSingleton(contextInterfaceType, sp => sp.GetService(contextDefaultImplType));
            }

            var assemblies = ModuleAssemblyHelper.GetAssemblies().ToList();
            services.AddAllModuleStartup(assemblies);
            
            configure?.Invoke(context);

            var provider = services.BuildServiceProvider();
            var startupModules = provider.GetServices<IModuleStartup>();
            startupModules = startupModules.OrderBy(x => x.Order);
            foreach (var startup in startupModules)
            {
                startup.ConfigureServices(services);
            }
            return context;
        }
        
        private static void AddAllModuleStartup(this IServiceCollection services, IList<Assembly> moduleAssemblies)
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
    }
}
