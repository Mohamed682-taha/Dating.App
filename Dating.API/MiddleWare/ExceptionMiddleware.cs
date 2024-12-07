using System.Net;
using System.Text.Json;
using Dating.API.Errors;

namespace Dating.API.MiddleWare;

public class ExceptionMiddleware(
    ILogger<ExceptionMiddleware> logger,
    RequestDelegate next,
    IWebHostEnvironment env
)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = env.IsDevelopment()
                ? new ApiServerErrorResponse(context.Response.StatusCode, ex.Message, ex.StackTrace)
                : new ApiServerErrorResponse(context.Response.StatusCode, ex.Message);
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }
}