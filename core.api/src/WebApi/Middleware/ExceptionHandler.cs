using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;

namespace WebApi.Middleware;


public class ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Proceed to the next middleware in the pipeline
            await next(context);
        }
        catch (Exception ex)
        {
            // Log the exception
            logger.LogError(ex, "An unhandled exception occurred.");

            // Handle the exception and return a generic BadRequest
            await HandleExceptionAsync(context);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "An unexpected error occurred. It's not you; It's us."
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}