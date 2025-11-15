using Domain.ApiContracts.Identity;
using Domain.Models.Entities.Identity;

namespace Infrastructure.Services.Interfaces;

public interface ITokenService
{
    Task<AccessTokenResponse> GenerateAccessToken(int userId, bool rememberMe = false);

    Task<AccessTokenResponse?> RefreshToken(string refreshToken);
    
}