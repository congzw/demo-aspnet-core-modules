using System;
using System.Linq;
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
            
            var context = services.LastOrDefault(d => d.ServiceType == typeof(IModuleServiceContext))?.ImplementationInstance as IModuleServiceContext;
            if (context == null)
            {
                context = new ModuleServiceContext()
                {
                    ApplicationServices = services
                };
                services.AddSingleton(serviceProvider => context);
            }

            if (context.Assemblies == null)
            {
                context.Assemblies = ModuleStartupHelper.GetModuleAssemblies().ToList();
            }
            ModuleStartupHelper.AddAllModuleStartup(services, context.Assemblies);
            
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
    }
}
