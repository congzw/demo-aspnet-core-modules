using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.Common.Modules.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMyModules(this IApplicationBuilder app, Action<IApplicationBuilder> configure = null)
        {
            var startUps = app.ApplicationServices.GetServices<IModuleStartup>();
            startUps = startUps.OrderBy(x => x.Order);
            foreach (var startup in startUps)
            {
                startup.Configure(app);
            }
            return app;
        }
    }
}
