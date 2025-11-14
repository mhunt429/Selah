using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Domain.ApiContracts.Identity;
using Domain.Configuration;
using Domain.Models.Entities.Identity;
using Infrastructure.Repository;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly SecurityConfig _securityConfig;
    private readonly TokenRepository _tokenRepository;
    private readonly ICryptoService _cryptoService;

    public TokenService(SecurityConfig securityConfig, TokenRepository tokenRepository, ICryptoService cryptoService)
    {
        _securityConfig = securityConfig;
        _tokenRepository = tokenRepository;
        _cryptoService = cryptoService;
    }

    public AccessTokenResponse GenerateAccessToken(int userId, bool rememberMe = false)
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
        string refreshToken = GenerateRefreshToken(userId);

        var sessionId = Guid.NewGuid();
        var sessionExpiration = DateTimeOffset.UtcNow.AddMinutes(30);


        return new AccessTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiration = new DateTimeOffset(accessTokenExpiration).ToUnixTimeMilliseconds(),
            RefreshTokenExpiration = DateTimeOffset.UtcNow.AddDays(_securityConfig.RefreshTokenExpiryDays)
                .ToUnixTimeMilliseconds(),
            SessionId = sessionId,
            SessionExpiration = sessionExpiration,
        };
    }

    public async Task<AccessTokenResponse?> RefreshToken(string refreshToken)
    {
        var refreshTokenParts = refreshToken.Split('|');
        if (refreshTokenParts.Length != 2)
        {
            return null;
        }

        var userId = int.Parse(refreshTokenParts[1]);

        TokenEntity? token = await _tokenRepository.GetTokenByUserId(userId, TokenType.RefreshToken);
        if (token == null) return null;

        string decryptedToken = _cryptoService.Decrypt(token.Token);

        if (refreshToken != decryptedToken)
        {
            return null;
        }

        return GenerateAccessToken(userId);
    }

    public string GenerateRefreshToken(int userId)
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return $"{Convert.ToBase64String(randomNumber)}|{userId}";
        }
    }
}