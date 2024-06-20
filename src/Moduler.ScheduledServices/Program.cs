using Autofac;
using Autofac.Extensions.DependencyInjection;
using Moduler.KuveytTurk.Services;
using Moduler.ScheduledServices.IoC;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

services.AddDbServices(configuration);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
var diModule = new MyApplicationModule(); 
diModule.AssemblyNameSpaces = configuration.GetValue<string>("AssembliesToRegister").ToString().Split(";");
builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(diModule));

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
