using Domain.ApiContracts.Banking;
using Domain.Models;
using Infrastructure.Repository.Interfaces;
using MediatR;

namespace Application.Banking;

public class GetAccountsByUserId : IRequest<ApiResponseResult<IEnumerable<FinancialAccountDto>>>
{
    public class Query : IRequest<ApiResponseResult<IEnumerable<FinancialAccountDto>>>
    {
        public int UserId { get; set; }
    }

    public class Handler : IRequestHandler<Query, ApiResponseResult<IEnumerable<FinancialAccountDto>>>
    {
        private readonly IFinancialAccountRepository _financialAccountRepository;

        public Handler(IFinancialAccountRepository financialAccountRepository)
        {
            _financialAccountRepository = financialAccountRepository;
        }

        public async Task<ApiResponseResult<IEnumerable<FinancialAccountDto>>> Handle(Query request,
            CancellationToken cancellationToken)
        {
            var dbAccounts = await _financialAccountRepository.GetAccountsAsync(request.UserId);

            var dto = dbAccounts.Select(x => new FinancialAccountDto
            {
                Id = x.Id,
                CurrentBalance = x.CurrentBalance,
                AccountMask = x.AccountMask,
                DisplayName = x.DisplayName,
                OfficialName = x.OfficialName,
                Subtype = x.Subtype,
                LastApiSyncTime = x.LastApiSyncTime
            });

            return new ApiResponseResult<IEnumerable<FinancialAccountDto>>(ResultStatus.Success, null, dto);
        }
    }
}