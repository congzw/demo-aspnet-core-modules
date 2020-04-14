using Microsoft.Extensions.DependencyInjection;

namespace MyApp.Common.Modules
{
    public interface IModuleServiceContext
    {
        IServiceCollection ApplicationServices { get; set; }
    }
}
