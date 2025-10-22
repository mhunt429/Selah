using Domain.Models.Entities.ApplicationUser;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;
using MediatR;

namespace Application.Identity;

/// <summary>
/// Handler for getting user by the sessionId from the HTTP context.
///
/// Returns null if the session is not found in the database or has expired
/// </summary>
public class UserBySessionIdQuery
{
    public class Query : IRequest<Domain.ApiContracts.ApplicationUser?>
    {
        public Guid SessionId { get; set; }
    }

    public class Handler : IRequestHandler<Query, Domain.ApiContracts.ApplicationUser?>
    {
        private readonly IUserSessionRepository _userSessionRepository;
        private ICryptoService _cryptoService;

        public Handler(IUserSessionRepository userSessionRepository, ICryptoService cryptoService)
        {
            _userSessionRepository = userSessionRepository;
            _cryptoService = cryptoService;
        }

        public async Task<Domain.ApiContracts.ApplicationUser?> Handle(Query query, CancellationToken cancellationToken)
        {
            ApplicationUserEntity? dbUser = await _userSessionRepository.GetUserByActiveSessionId(query.SessionId);
            if (dbUser == null) return null;

            var nameParts = _cryptoService.Decrypt(dbUser.EncryptedName).Split("|");
            return new Domain.ApiContracts.ApplicationUser
            {
                Id = dbUser.Id,
                AccountId = dbUser.AccountId,
                Email = _cryptoService.Decrypt(dbUser.EncryptedEmail),
                FirstName = nameParts[0],
                LastName = nameParts[1],
                CreatedDate = dbUser.CreatedDate
            };
        }
    }
}