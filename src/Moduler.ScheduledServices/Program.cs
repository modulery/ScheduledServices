using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.PostgreSql;
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

services.AddHangfire(x => x.UsePostgreSqlStorage(configuration.GetConnectionString("HangfireDbContext")));
services.AddHangfireServer();

services.AddControllersWithViews();

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

app.UseHangfireDashboard("/automations");
app.UseHangfireServer();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
