using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.Common.Modules
{
    public interface IModuleStartup
    {
        /// <summary>
        /// 执行顺序 越小越靠前
        /// </summary>
        int Order { get; }

        void ConfigureServices(IServiceCollection services);

        void Configure(IApplicationBuilder builder);
    }

    public abstract class ModuleStartupBase : IModuleStartup
    {
        /// <inheritdoc />
        public virtual int Order { get; } = 0;

        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {
        }

        /// <inheritdoc />
        public virtual void Configure(IApplicationBuilder app)
        {
        }
    }
}
