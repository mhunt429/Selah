using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Domain.Models;

namespace WebApi.Extensions;

public static class HttpRequestExtensions
{
    public static AppRequestContext GetAppRequestContext(this HttpRequest request)
    {
        string? token = string.Empty;
        string? forwardedFor = request.Headers["X-Forwarded-For"].FirstOrDefault();
        string? ipAddress = !string.IsNullOrWhiteSpace(forwardedFor)
            ? forwardedFor.Split(',').FirstOrDefault()?.Trim()
            : request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        token = request.Headers["Authorization"]
            .FirstOrDefault(h => h != null && h.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase))
            ?.Substring("Bearer ".Length);

        if (string.IsNullOrWhiteSpace(token))
        {
            request.Cookies.TryGetValue("x_api_token", out token!);
        }

        int userId = GetUserIdFromToken(token!);

        return new AppRequestContext
        {
            IpAddress = ipAddress ?? "",
            UserId = userId,
            TraceId = request.HttpContext.TraceIdentifier,
        };
    }

    private static int GetUserIdFromToken(string token)
    {
        int userId = -1;
        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);
                string? subjectValue = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                int.TryParse(subjectValue, out userId);
                return userId;
            }
        }
        catch
        {
            return -1;
        }

        return -1;
    }
}