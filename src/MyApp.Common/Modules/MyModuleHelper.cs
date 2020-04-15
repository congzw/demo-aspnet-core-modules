using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using MyApp.Common.Modules.Impl;

namespace MyApp.Common.Modules
{
    public class MyModuleHelper
    {
        public MyModuleHelper()
        {
            LoadContext = AssemblyLoadContext.Default;
            GetAssemblies = () => GetModuleAssemblies(AppDomain.CurrentDomain.BaseDirectory);
            //LoadContext.Resolving += LoadContext_Resolving;
        }

        //private Assembly LoadContext_Resolving(AssemblyLoadContext loadContext, AssemblyName assemblyName)
        //{
        //    //todo
        //    //The context also provides a Resolving-event which could be used to bring in your resolving code,
        //    //if an assembly could not be resolved byte the LoadContext itself.

        //    //Maybe Load from different path e.g. Addon Path.
        //    return loadContext.LoadFromAssemblyPath(@"C:\Addons\" + assemblyName.Name + ".dll");
        //}

        public AssemblyLoadContext LoadContext { get; set; }
        
        public Func<IList<Assembly>> GetAssemblies { get; set; }

        public IDictionary<string, string> AddApplicationPart(IMvcBuilder mvcBuilder)
        {
            var loadResult = new Dictionary<string, string>();
            var assemblies = GetAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    //for test loadResult
                    var asm = LoadContext.LoadFromAssemblyName(assembly.GetName());

                    mvcBuilder.AddApplicationPart(asm);
                    loadResult[assembly.FullName] = "OK";
                }
                catch (Exception ex)
                {
                    loadResult[assembly.FullName] = "KO: " + ex.Message;
                }
            }

            return loadResult;
        }

        public static MyModuleHelper Instance = new MyModuleHelper();
        private IList<Assembly> GetModuleAssemblies(string root, string modulePrefix = null)
        {
            if (string.IsNullOrWhiteSpace(modulePrefix))
            {
                modulePrefix = TryGetPrefix();
            }

            if (string.IsNullOrWhiteSpace(modulePrefix))
            {
                throw new ArgumentNullException(nameof(modulePrefix));
            }

            var allLib = DependencyContext.Default.CompileLibraries;
            var libs = allLib.Where(x => x.Name.StartsWith(modulePrefix, StringComparison.OrdinalIgnoreCase));
            var assemblies = libs.Select(lib =>
                {
                    try
                    {
                        return LoadContext.LoadFromAssemblyName(new AssemblyName(lib.Name));
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            ).Where(x => x != null).ToList();

            var moduleFiles = Directory.GetFiles(root, modulePrefix + "*.dll");
            foreach (var moduleFile in moduleFiles)
            {
                var fileInfo = new FileInfo(moduleFile);
                var theOne = assemblies.SingleOrDefault(x => fileInfo.Name.Equals(x.GetName().Name + ".dll", StringComparison.OrdinalIgnoreCase));
                if (theOne == null)
                {
                    var assembly = LoadContext.LoadFromAssemblyPath(fileInfo.FullName);
                    assemblies.Add(assembly);
                }
            }

            return assemblies;
        }
        private static string TryGetPrefix()
        {
            var ns = typeof(ModuleServiceContext).Namespace;
            if (ns != null)
            {
                var modulePrefix = ns.Split(".").FirstOrDefault();
                return modulePrefix;
            }
            //todo: read from config?
            return "";
        }
    }
}