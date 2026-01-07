using Domain.ApiContracts.Identity;
using Domain.Models.Entities.ApplicationUser;
using Domain.Models.Entities.Identity;
using Domain.Results;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;

namespace Application.Services;

public class IdentityService
{
    private readonly IApplicationUserRepository _userRepository;
    private readonly ICryptoService _cryptoService;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly ITokenService _tokenService;
    private readonly IUserSessionRepository _userSessionRepository;

    public IdentityService(IApplicationUserRepository userRepository, ICryptoService cryptoService,
        IPasswordHasherService passwordHasherService, ITokenService tokenService,
        IUserSessionRepository userSessionRepository)
    {
        _userRepository = userRepository;
        _cryptoService = cryptoService;
        _passwordHasherService = passwordHasherService;
        _tokenService = tokenService;
        _userSessionRepository = userSessionRepository;
    }


    public async Task<LoginResult> Login(LoginRequest request)
    {
        string hashedEmail = _cryptoService.HashValue(request.Email);
        ApplicationUserEntity? dbUser = await _userRepository.GetUserByEmail(hashedEmail);

        if (dbUser == null) return new LoginResult(LoginStatus.Failed, null);

        if (_passwordHasherService.VerifyPassword(request.Password, dbUser.Password))
        {
            var sessionId = Guid.NewGuid();
            var sessionExpiration = request.RememberMe
                ? DateTimeOffset.UtcNow.AddDays(7)
                : DateTimeOffset.UtcNow.AddMinutes(30);

            await _userSessionRepository.IssueSession(new UserSessionEntity
            {
                Id = sessionId,
                AppLastChangedBy = dbUser.Id,
                UserId = dbUser.Id,
                IssuedAt = DateTimeOffset.UtcNow,
                ExpiresAt = sessionExpiration
            });
            AccessTokenResponse rsp = await _tokenService.GenerateAccessToken(dbUser.Id, request.RememberMe);
            return new LoginResult(LoginStatus.Success, rsp);
        }

        return new LoginResult(LoginStatus.Failed, null);
    }
    
    
    public async Task<LoginResult> RefreshAccessToken(RefreshTokenRequest request)
    {
        AccessTokenResponse? rsp = await _tokenService.RefreshToken(request.RefreshToken);
        if (rsp is null) return new LoginResult(LoginStatus.Failed, null);

        return new LoginResult(LoginStatus.Success, rsp);
    }
}