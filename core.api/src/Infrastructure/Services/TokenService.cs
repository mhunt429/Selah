using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Domain.ApiContracts.Identity;
using Domain.Configuration;
using Infrastructure.Services.Interfaces;

namespace Infrastructure;

public class TokenService : ITokenService
{
    private readonly SecurityConfig _securityConfig;

    public TokenService(SecurityConfig securityConfig)
    {
        _securityConfig = securityConfig;
    }

    public AccessTokenResponse GenerateAccessToken(int userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.UTF8.GetBytes(_securityConfig.JwtSecret);

        DateTime accessTokenExpiration = DateTime.UtcNow.AddMinutes(_securityConfig.AccessTokenExpiryMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("sub", userId.ToString()),
            }),
            Expires = accessTokenExpiration,
            Issuer = "selah-api",
            Audience = "selah-api",
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };

        SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);

        string accessToken = tokenHandler.WriteToken(token);
        string refreshToken = GenerateRefreshToken();

        return new AccessTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiration = accessTokenExpiration,
            RefreshTokenExpiration = DateTime.UtcNow.AddDays(_securityConfig.RefreshTokenExpiryDays)
        };
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}