using Microsoft.Extensions.DependencyInjection;

namespace MyApp.Common.Modules.Impl
{
    public class ModuleServiceContext : IModuleServiceContext
    {
        public IServiceCollection ApplicationServices { get; set; }
    }
}
