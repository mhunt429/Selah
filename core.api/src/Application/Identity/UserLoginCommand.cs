using MediatR;
using Domain.ApiContracts.Identity;
using Domain.Models.Entities.ApplicationUser;
using Domain.Models.Entities.Identity;
using Domain.Results;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;

namespace Application.Identity;

public class UserLogin
{
    public class Command : LoginRequest, IRequest<LoginResult>
    {
    }


    public class Handler : IRequestHandler<Command, LoginResult>
    {
        private readonly IApplicationUserRepository _repository;
        private readonly ICryptoService _cryptoService;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly ITokenService _tokenService;
        private readonly IUserSessionRepository _userSessionRepository;


        public Handler(
            IApplicationUserRepository repository,
            ICryptoService cryptoService,
            IPasswordHasherService passwordHasherService,
            ITokenService tokenService,
            IUserSessionRepository userSessionRepository)
        {
            _repository = repository;
            _cryptoService = cryptoService;
            _passwordHasherService = passwordHasherService;
            _tokenService = tokenService;
            _userSessionRepository = userSessionRepository;
        }

        public async Task<LoginResult> Handle(Command command, CancellationToken cancellationToken)
        {
            string hashedEmail = _cryptoService.HashValue(command.Email);
            ApplicationUserEntity? dbUser = await _repository.GetUserByEmail(hashedEmail);

            if (dbUser == null) return new LoginResult(false, null);

            if (_passwordHasherService.VerifyPassword(command.Password, dbUser.Password))
            {
                var sessionId = Guid.NewGuid();
                var sessionExpiration = command.RememberMe
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
                AccessTokenResponse rsp = await _tokenService.GenerateAccessToken(dbUser.Id, command.RememberMe);
                return new LoginResult(true, rsp);
            }

            return new LoginResult(false, null);
        }
    }
}