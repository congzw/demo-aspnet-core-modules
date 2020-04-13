using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.Common.Modules.ModuleA
{
    public interface ILifetimeDesc
    {

    }

    public interface IMySingletonDesc : ILifetimeDesc
    {

    }

    public interface IMyScopedDesc : ILifetimeDesc
    {

    }

    public interface IMyTransientDesc : ILifetimeDesc
    {

    }

    public class LifetimeDesc : IMySingletonDesc, IMyScopedDesc, IMyTransientDesc
    {
        public override string ToString()
        {
            return this.GetHashCode().ToString();
        }

        public static string ShowDiff(ILifetimeDesc desc, ILifetimeDesc desc2)
        {
            return string.Format("[{0}, {1}] Same: {2}"
                , desc == null ? "NULL" : desc.ToString()
                , desc2 == null ? "NULL" : desc2.ToString()
                , object.ReferenceEquals(desc, desc2));
        }
    }

    public class ModuleAStartup : IModuleStartup
    {
        public ModuleAStartup()
        {
            Order = 1;
        }

        public int Order { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IMyTransientDesc, LifetimeDesc>();
            services.AddScoped<IMyScopedDesc, LifetimeDesc>();
            services.AddSingleton<IMySingletonDesc, LifetimeDesc>();
        }

        public void Configure(IApplicationBuilder builder)
        {
        }
    }
}
