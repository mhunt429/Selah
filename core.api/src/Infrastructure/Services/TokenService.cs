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

public class TokenService(SecurityConfig securityConfig, TokenRepository tokenRepository, ICryptoService cryptoService)
    : ITokenService
{
    public async Task<AccessTokenResponse> GenerateAccessToken(int userId, bool rememberMe = false)
    {

        DateTime accessTokenExpiration = DateTime.UtcNow.AddMinutes(securityConfig.AccessTokenExpiryMinutes);
        
        var claims = new List<Claim>(){new(JwtRegisteredClaimNames.Sub, userId.ToString())};

        string accessToken = cryptoService.GenerateJwt(claims, accessTokenExpiration);

        string refreshToken = Guid.NewGuid().ToString();

        var sessionId = Guid.NewGuid();
        var sessionExpiration = DateTimeOffset.UtcNow.AddMinutes(30);

        var refreshTokenExpiration = DateTimeOffset.UtcNow.AddDays(securityConfig.RefreshTokenExpiryDays);

        await tokenRepository.SaveTokenAsync(new TokenEntity
        {
            AppLastChangedBy = userId,
            UserId = userId,
            Token = cryptoService.HashValue(refreshToken),
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
        string hashedToken = cryptoService.HashValue(refreshToken);

        TokenEntity? tokenDb = await tokenRepository.GetByTokenHash(hashedToken, TokenType.RefreshToken);
        if (tokenDb == null) return null;


        if (hashedToken != tokenDb.Token)
        {
            return null;
        }

        return await GenerateAccessToken(tokenDb.UserId);
    }
}