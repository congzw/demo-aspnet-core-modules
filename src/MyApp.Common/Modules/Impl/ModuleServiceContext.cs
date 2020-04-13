using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.Common.Modules.Impl
{
    public class ModuleServiceContext : IModuleServiceContext
    {
        public ModuleServiceContext()
        {
        }

        public IServiceCollection ApplicationServices { get; set; }
        public IEnumerable<Assembly> Assemblies { get; set; }
    }
}
