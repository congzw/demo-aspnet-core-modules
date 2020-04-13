using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.Common.Modules
{
    public interface IModuleServiceContext
    {
        IServiceCollection ApplicationServices { get; set; }

        IEnumerable<Assembly> Assemblies { get; set; }
    }
}
