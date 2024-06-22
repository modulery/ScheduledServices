using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.OpenApi.Models;
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
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.DocumentFilter<DynamicApiDocumentFilter>();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseHangfireDashboard("/automations");
app.UseHangfireServer();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseMiddleware<DynamicApiMiddleware>();

app.Run();
