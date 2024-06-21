using Autofac;
using RestSharp;
using System.Reflection;

namespace Moduler.ScheduledServices.IoC
{
    public class MyApplicationModule : Autofac.Module
    {
        public string[] AssemblyNameSpaces { get; set; }
        public static Type[] MyInterfaces { get; private set; } = new Type[0];
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

                    Type[] types = assembly.GetTypes();
                    Type[] interfaces = types.Where(t => t.IsInterface).ToArray();
                    if (interfaces.Length>0)
                    {
                        MyInterfaces = (Type[])MyInterfaces.Concat(interfaces).ToArray();
                        foreach (Type interfaceType in interfaces)
                        {
                            Console.WriteLine(interfaceType.Name);
                            foreach (var method in interfaceType.GetMethods())
                            {
                                var returnType = method.ReturnType.FullName;
                                var parameters = method.GetParameters();
                            }
                        }
                    }

                    builder.RegisterAssemblyTypes(assembly)
                        .Where(t => t.Namespace.StartsWith(assemblyName) && t.IsInterface)
                        .AsImplementedInterfaces();
                }
            }
        }
    }
}