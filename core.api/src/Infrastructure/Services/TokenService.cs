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

    public async Task<AccessTokenResponse> GenerateAccessToken(int userId, bool rememberMe = false)
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
        string refreshToken = Guid.NewGuid().ToString();

        var sessionId = Guid.NewGuid();
        var sessionExpiration = DateTimeOffset.UtcNow.AddMinutes(30);

        var refreshTokenExpiration = DateTimeOffset.UtcNow.AddDays(_securityConfig.RefreshTokenExpiryDays);

        await _tokenRepository.SaveTokenAsync(new TokenEntity
        {
            UserId = userId,
            Token = _cryptoService.HashValue(refreshToken),
            TokenType = TokenType.RefreshToken,
            ExpiresAt = refreshTokenExpiration,
            CreatedAt = DateTimeOffset.UtcNow
        });

        return new AccessTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiration = new DateTimeOffset(accessTokenExpiration).ToUnixTimeMilliseconds(),
            RefreshTokenExpiration = refreshTokenExpiration.ToUnixTimeMilliseconds(),
            SessionId = sessionId,
            SessionExpiration = sessionExpiration,
        };
    }

    public async Task<AccessTokenResponse?> RefreshToken(string refreshToken)
    {
        string hashedToken = _cryptoService.HashValue(refreshToken);

        TokenEntity? tokenDb = await _tokenRepository.GetByTokenHash(hashedToken, TokenType.RefreshToken);
        if (tokenDb == null) return null;


        if (hashedToken != tokenDb.Token)
        {
            return null;
        }

        return await GenerateAccessToken(tokenDb.UserId);
    }
}