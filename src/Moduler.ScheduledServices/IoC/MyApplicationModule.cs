using Autofac;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Reflection;

namespace Moduler.ScheduledServices.IoC
{
    public class MyApplicationModule : Autofac.Module
    {
        public string[] AssemblyNameSpaces { get; set; }
        public static Type[] MyInterfaces { get; private set; } = new Type[0];
        public static Type[] MyServices { get; private set; } = new Type[0];

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
                    Type[] services = interfaces.SelectMany(interfaceType => types.Where(t => t.IsClass && !t.IsAbstract && interfaceType.IsAssignableFrom(t))).ToArray();

                    if (interfaces.Length > 0)
                    {
                        MyInterfaces = (Type[])MyInterfaces.Concat(interfaces).ToArray();
                        foreach (Type interfaceType in interfaces)
                        {
                            var serviceType = services.FirstOrDefault(t => interfaceType.IsAssignableFrom(t));
                            if (serviceType == null) continue;

                            foreach (var method in interfaceType.GetMethods())
                            {
                                var returnType = method.ReturnType;
                                var parameters = method.GetParameters();
                                Console.WriteLine(interfaceType.Name + " " + serviceType.Name + " " + method.Name);
                            }

                            Console.WriteLine(interfaceType.Name + " " + serviceType.Name);
                            MyServices = (Type[])MyServices.Concat(new Type[] { serviceType }).ToArray();
                            try
                            {
                                builder.RegisterType(serviceType)
                                    .As(interfaceType)
                                    .InstancePerLifetimeScope();
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    Console.WriteLine("----------------------");
                }
            }
        }
    }
}