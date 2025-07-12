using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Domain.Models;

namespace WebApi.Extensions;

[ExcludeFromCodeCoverage]
public static class HttpRequestExtensions
{
    public static AppRequestContext GetAppRequestContext(this HttpRequest request)
    {
        string? forwardedFor = request.Headers["X-Forwarded-For"].FirstOrDefault();
        string? ipAddress = !string.IsNullOrWhiteSpace(forwardedFor)
            ? forwardedFor.Split(',').FirstOrDefault()?.Trim()
            : request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        string? bearerToken = request.Headers["Authorization"]
            .FirstOrDefault(h => h.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase))
            ?.Substring("Bearer ".Length);
        
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            request.Cookies.TryGetValue("x_token", out bearerToken);
        }
        
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            return new AppRequestContext
            {
                UserId = Guid.Empty,
            };
        }
        
        Guid userId = GetUserIdFromToken(bearerToken);

        return new AppRequestContext
        {
            IpAddress = ipAddress ?? "",
            UserId = userId,
            TraceId = request.HttpContext.TraceIdentifier,
        };
    }
    
    private static Guid GetUserIdFromToken(string token)
    {
        Guid userId;
        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);
                string subjectValue =  jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                Guid.TryParse(subjectValue, out userId);
                return userId;
            }
        }
        catch
        {
            return Guid.Empty;
        }

        return Guid.Empty; // Return null if extraction fails
    }
}