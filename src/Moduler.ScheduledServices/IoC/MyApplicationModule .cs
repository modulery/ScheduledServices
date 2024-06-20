using Autofac;
using System.Reflection;

namespace Moduler.ScheduledServices.IoC
{
    public class MyApplicationModule : Autofac.Module
    {
        public string[] AssemblyNameSpaces { get; set; }
        protected override void Load(ContainerBuilder builder)
        {
            var projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            foreach (var assemblyName in AssemblyNameSpaces.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray())
            {
                var assemblies = Directory.GetFiles(projectDirectory, assemblyName + "*.dll");

                foreach (var assemblyPath in assemblies)
                {
                    var assembly = Assembly.LoadFrom(assemblyPath);
                    Console.WriteLine(assembly.FullName);

                    builder.RegisterAssemblyTypes(assembly)
                        .Where(t => t.Namespace.StartsWith(assemblyName) && t.IsInterface)
                        .AsImplementedInterfaces();
                }
            }
        }
    }
}