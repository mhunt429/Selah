using Domain.ApiContracts.Identity;

namespace Infrastructure.Services.Interfaces;

public interface ITokenService
{
    AccessTokenResponse GenerateAccessToken(int userId, bool rememberMe = false);

    Task<AccessTokenResponse?> RefreshToken(string refreshToken);
}