using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace WebApi.Middleware;

public class JwtMiddleware : JwtBearerHandler
{
    public JwtMiddleware(
        IOptionsMonitor<JwtBearerOptions> options,
        ILoggerFactory logger,
        System.Text.Encodings.Web.UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string token = null;

        if (Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var bearerToken = authHeader.ToString();
            if (bearerToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = bearerToken.Substring("Bearer ".Length).Trim();
            }
        }

        if (string.IsNullOrEmpty(token))
        {
            token = Request.Cookies["x_api_token"];
        }

        // If no token is found, return failure
        if (string.IsNullOrEmpty(token))
        {
            return AuthenticateResult.Fail("No token provided");
        }

        try
        {
            var validationParameters = Options.TokenValidationParameters;
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is JwtSecurityToken jwt)
            {
                return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
            }

            return AuthenticateResult.Fail("Invalid token");
        }
        catch (Exception ex)
        {
            return AuthenticateResult.Fail($"Token validation failed: {ex.Message}");
        }
    }
}