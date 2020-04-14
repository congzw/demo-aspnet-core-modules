using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Common.Modules.Extensions;

namespace MyApp.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            ////todo figure out why default not find dynamic dll?
            //var demoDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyApp.Module.Demos.dll");
            //var demoDll = Assembly.LoadFile(demoDllPath);
            //var assemblies = ModuleAssemblyHelper.GetAssemblies().ToList();
            //assemblies.Add(demoDll);
            //ModuleAssemblyHelper.GetAssemblies = () => assemblies;

            //services.AddMyModules();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            //add[Area("Default")] in controllers of area
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
