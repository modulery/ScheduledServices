using Microsoft.OpenApi.Models;
using Moduler.ScheduledServices.IoC;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

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

        var parameters = method.GetParameters();
        var parameterValues = new object[parameters.Length];

        if (context.Request.Method == "POST" && context.Request.ContentType == "application/json")
        {
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            var paramDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);

            var parameter = parameters[0];
            parameterValues[0] = JsonConvert.DeserializeObject(body, parameter.ParameterType);
        }
        else
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var parameterType = parameter.ParameterType;

                if (context.Request.Query.TryGetValue(parameter.Name, out var queryValue))
                {
                    parameterValues[i] = JsonConvert.DeserializeObject(queryValue, parameterType);
                }
                else
                {
                    parameterValues[i] = parameter.HasDefaultValue ? parameter.DefaultValue : Activator.CreateInstance(parameterType);
                }
            }
        }

        var result = method.Invoke(service, parameterValues);
        var jsonResult = JsonConvert.SerializeObject(result);

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(jsonResult);
    }
}



public class DynamicApiDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var interfaces = MyApplicationModule.MyInterfaces;
        foreach (var interfaceType in interfaces)
        {
            var serviceName = interfaceType.Name;
            var servicePath = $"/DynamicApiTest/{serviceName}";

            foreach (var method in interfaceType.GetMethods())
            {
                var operation = new OpenApiOperation
                {
                    Tags = new List<OpenApiTag> { new OpenApiTag { Name = serviceName } },
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse { Description = "OK" }
                    }
                };

                var pathItem = new OpenApiPathItem();
                if (method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType.IsClass)
                {
                    var requestBody = new OpenApiRequestBody
                    {
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = context.SchemaGenerator.GenerateSchema(method.GetParameters().FirstOrDefault()?.ParameterType, context.SchemaRepository)
                            }
                        }
                    };
                    operation.RequestBody = requestBody;
                    pathItem.AddOperation(OperationType.Post, operation);
                }
                else
                {
                    foreach (var parameter in method.GetParameters())
                    {
                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = parameter.Name,
                            In = ParameterLocation.Query,
                            Required = !parameter.IsOptional,
                            Schema = context.SchemaGenerator.GenerateSchema(parameter.ParameterType, context.SchemaRepository)
                        });
                    }
                    pathItem.AddOperation(OperationType.Get, operation);
                }

                try
                {
                    swaggerDoc.Paths.Add($"{servicePath}/{method.Name}", pathItem);
                }
                catch (Exception)
                {
                    // Handle exception
                }
            }
        }
    }
}