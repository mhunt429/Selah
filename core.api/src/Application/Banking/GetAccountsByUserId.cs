using Domain.ApiContracts.Banking;
using Domain.Models;
using Infrastructure.Repository.Interfaces;
using MediatR;

namespace Application.Banking;

public class GetAccountsByUserId : IRequest<IEnumerable<FinancialAccountDto>>
{
    public class Query : IRequest<IEnumerable<FinancialAccountDto>>
    {
        public int UserId { get; set; }
    }

    public class Handler : IRequestHandler<Query, IEnumerable<FinancialAccountDto>>
    {
        private readonly IFinancialAccountRepository _financialAccountRepository;

        public Handler(IFinancialAccountRepository financialAccountRepository)
        {
            _financialAccountRepository = financialAccountRepository;
        }

        public async Task<IEnumerable<FinancialAccountDto>> Handle(Query request,
            CancellationToken cancellationToken)
        {
            var dbAccounts = await _financialAccountRepository.GetAccountsAsync(request.UserId);

            return dbAccounts.Select(x => new FinancialAccountDto
            {
                Id = x.Id,
                CurrentBalance = x.CurrentBalance,
                AccountMask = x.AccountMask,
                DisplayName = x.DisplayName,
                OfficialName = x.OfficialName,
                Subtype = x.Subtype,
                LastApiSyncTime = x.LastApiSyncTime
            });
        }
    }
}