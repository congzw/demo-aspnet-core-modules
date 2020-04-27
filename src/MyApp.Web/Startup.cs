using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyApp.Common.Modules.Extensions;

namespace MyApp.Web
{
    public class Startup
    {
        public ILogger<Startup> Logger { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(ILogger<Startup> logger, IHostingEnvironment hostingEnvironment)
        {
            Logger = logger;
            Environment = hostingEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddMyModules();

            var mvcBuilder = services.AddMvc();
            mvcBuilder.AddMyModulePart();
            mvcBuilder.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

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

        private void Log(string msg)
        {
            Logger.LogInformation(msg);
        }
    }
}
