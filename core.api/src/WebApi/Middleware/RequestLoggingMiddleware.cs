using System.Text;

namespace WebApi.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString();
        var method = context.Request.Method;
        var path = context.Request.Path;
        
        // Store the original response body stream
        var originalBodyStream = context.Response.Body;
        
        // Create a new memory stream to capture the response
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        
        await _next(context);

        var statusCode = context.Response.StatusCode;
        string? responseBodyText = null;

        if (statusCode >= 400)
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();
            responseBody.Seek(0, SeekOrigin.Begin);
            
            // Copy the response body back to the original stream
            await responseBody.CopyToAsync(originalBodyStream);
            _logger.LogWarning("HTTP {Method} {Path} from {IP} => {StatusCode} | Response: {ResponseBody}",
                method, path, ip, statusCode, responseBodyText);
        }
        
        else
        {
            _logger.LogInformation("HTTP {Method} {Path} from {IP} => {StatusCode}",
                method, path, ip, statusCode);
        }
    }
}