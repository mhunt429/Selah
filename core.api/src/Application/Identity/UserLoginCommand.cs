using MediatR;
using Domain.ApiContracts.Identity;
using Domain.Models.Entities.ApplicationUser;
using Domain.Models.Entities.Identity;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;

namespace Application.Identity;

public class UserLogin
{
    public class Command : LoginRequest, IRequest<Command.Response>
    {

        public class Response
        {
            public AccessTokenResponse? AccessToken { get; set; }
            
            //Send these back to the controller so the sessionId cookie can be issues
            public Guid SessionId { get; set; }
            
            public DateTimeOffset SessionExpiration { get; set; }
        }
        public class Handler : IRequestHandler<Command, Response>
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

            public async Task<Response> Handle(Command command, CancellationToken cancellationToken)
            {
                string hashedEmail = _cryptoService.HashValue(command.Email);
                ApplicationUserEntity? dbUser = await _repository.GetUserByEmail(hashedEmail);

                if (dbUser == null) return null;

                if (_passwordHasherService.VerifyPassword(command.Password, dbUser.Password))
                {
                    var sessionId = Guid.NewGuid();
                    var sessionExpiration = DateTimeOffset.UtcNow.AddDays(7);
                    
                    await _userSessionRepository.IssueSession(new UserSessionEntity
                    {
                        AppLastChangedBy = dbUser.Id,
                        UserId = dbUser.Id,
                        SessionId =sessionId,
                        IssuedAt = DateTimeOffset.UtcNow,
                        ExpiresAt = sessionExpiration,
                        OriginalInsert = DateTimeOffset.UtcNow
                    });


                    var response = new Response
                    {
                        AccessToken = _tokenService.GenerateAccessToken(dbUser.Id),
                        SessionId = sessionId,
                        SessionExpiration = sessionExpiration,
                    };
                    
                    return response;
                }

                return null;
            }
        }
    }
}