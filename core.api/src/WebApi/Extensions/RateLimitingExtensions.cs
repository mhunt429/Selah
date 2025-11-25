using System.Threading.RateLimiting;

namespace WebApi.Extensions;

public static class RateLimitingExtensions
{
    public static void RegisterRateLimiter(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy("PublicEndpointPolicy", context =>
            {
                var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                         ?? context.Connection.RemoteIpAddress?.ToString()
                         ?? "unknown";
                return RateLimitPartition.GetSlidingWindowLimiter(
                    ip,
                    _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = env.EnvironmentName == "IntegrationTests" ? 3 : 20,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 4,
                        QueueLimit = 0
                    });
            });

            options.AddPolicy("UserTokenPolicy", context =>
            {
                var accessToken = context.Request.Cookies["x_api_token"] ?? "anonymous";
                return RateLimitPartition.GetTokenBucketLimiter(
                    accessToken,
                    _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = env.EnvironmentName == "IntegrationTests" ? 3 : 100,
                        TokensPerPeriod = 10,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    });
            });
        });
    }
}