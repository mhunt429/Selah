using Application.Mappings;
using Domain.Models.Entities.ApplicationUser;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;

namespace Application.Services;

public class AppUserService
{
    private readonly ICryptoService _cryptoService;
    private readonly IApplicationUserRepository _userRepository;

    public AppUserService(IApplicationUserRepository userRepository, ICryptoService cryptoService)
    {
        _cryptoService = cryptoService;
        _userRepository = userRepository;
    }

    public async Task<Domain.ApiContracts.ApplicationUser> GetUserById(int userId)
    {
        ApplicationUserEntity? userSql = await _userRepository.GetUserByIdAsync(userId);
        if (userSql == null) return null!;

        return userSql.MapAppUserDataAccessToApiContract(_cryptoService);
    }
}