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

        var originalBody = context.Response.Body;
        await using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        await _next(context);

        buffer.Position = 0;

        var statusCode = context.Response.StatusCode;

        if (statusCode >= 400)
        {
            var responseText = await new StreamReader(buffer, leaveOpen: true).ReadToEndAsync();
            buffer.Position = 0;

            _logger.LogWarning(
                "HTTP {Method} {Path} from {IP} => {StatusCode} | Response: {ResponseBody}",
                method, path, ip, statusCode, responseText
            );
        }
        else
        {
            _logger.LogInformation(
                "HTTP {Method} {Path} from {IP} => {StatusCode}",
                method, path, ip, statusCode
            );
        }

        context.Response.Body = originalBody;
        await buffer.CopyToAsync(originalBody);
    }
}