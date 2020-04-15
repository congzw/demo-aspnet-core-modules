using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyApp.Common.Modules.Extensions;
using MyApp.Common.Modules.ModuleA;

namespace MyApp.Common.Modules
{
    [TestClass]
    public class ModuleStartupSpec
    {
        static IServiceCollection services;
        static IServiceProvider rootProvider;
        static IApplicationBuilder builder;

        [ClassInitialize]
        public static void ModuleStartupInitialize(TestContext testContext)
        {
            services = new ServiceCollection();
            services.AddMyModules(new[]{ typeof(ModuleStartupSpec).Assembly });
            rootProvider = services.BuildServiceProvider();
            builder = new ApplicationBuilder(rootProvider);
            builder.Build();
        }

        [TestMethod]
        public void AddMyModules_Singleton_Should_Ok()
        {
            var service = rootProvider.GetService<IMySingletonDesc>();
            service.ShouldNotNull();
            var service2 = rootProvider.GetService<IMySingletonDesc>();
            service2.ShouldNotNull();

            service2.ShouldEqual(service);
        }

        [TestMethod]
        public void AddMyModules_Scoped_Should_Ok()
        {
            using (var scope = rootProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetService<IMyScopedDesc>();
                service.ShouldNotNull();
                var service2 = scope.ServiceProvider.GetService<IMyScopedDesc>();
                service2.ShouldNotNull();

                service2.ShouldEqual(service);

                using (var scope2 = rootProvider.CreateScope())
                {
                    var service3 = scope2.ServiceProvider.GetService<IMyScopedDesc>();
                    service3.ShouldNotNull();

                    service3.ShouldNotEqual(service);
                }
            }
        }
        
        [TestMethod]
        public void AddMyModules_Transient_Should_Ok()
        {
            var service = rootProvider.GetService<IMyTransientDesc>();
            service.ShouldNotNull();
            var service2 = rootProvider.GetService<IMyScopedDesc>();
            service2.ShouldNotNull();
            service2.ShouldNotEqual(service);
        }
    }
}
