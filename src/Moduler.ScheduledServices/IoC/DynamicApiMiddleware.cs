using Moduler.ScheduledServices.IoC;

public class DynamicApiMiddleware
{
    private readonly RequestDelegate _next;

    public DynamicApiMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value.Trim('/');
        var segments = path.Split('/');

        if (segments.Length < 3 || segments[0] != "DynamicApiTest")
        {
            await _next(context);
            return;
        }

        var serviceName = segments[1];
        var methodName = segments[2];

        var serviceType = MyApplicationModule.MyInterfaces.FirstOrDefault(t => t.Name == serviceName);
        if (serviceType == null)
        {
            await _next(context);
            return;
        }

        var service = context.RequestServices.GetService(serviceType);
        if (service == null)
        {
            await _next(context);
            return;
        }

        var method = serviceType.GetMethod(methodName);
        if (method == null)
        {
            await _next(context);
            return;
        }

        var result = method.Invoke(service, null);
        var jsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(result);

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(jsonResult);
    }
}
