using Application.Mappings;
using Domain.Models.Entities.ApplicationUser;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;
using MediatR;

namespace Application.ApplicationUser;

public class GetUserById
{
    public class Query : IRequest<Domain.ApiContracts.ApplicationUser>
    {
        public int UserId { get; set; }
    }

    public class Handler : IRequestHandler<Query, Domain.ApiContracts.ApplicationUser>
    {
        private readonly ICryptoService _cryptoService;
        private readonly IApplicationUserRepository _repository;

        public Handler(IApplicationUserRepository repository, ICryptoService cryptoService)
        {
            _repository = repository;
            _cryptoService = cryptoService;
        }

        public async Task<Domain.ApiContracts.ApplicationUser> Handle(Query query, CancellationToken cancellationToken)
        {
            ApplicationUserEntity? userSql = await _repository.GetUserByIdAsync(query.UserId);
            if (userSql == null) return null!;

            return userSql.MapAppUserDataAccessToApiContract(_cryptoService);
        }
    }
}