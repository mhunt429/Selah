using Domain.ApiContracts.Banking;
using Domain.Models.Entities.FinancialAccount;
using Infrastructure.Repository.Interfaces;

namespace Application.Services;

public class BankingService(IFinancialAccountRepository financialAccountRepository)
{
    public async Task<IEnumerable<FinancialAccountDto>> GetAccountsByUserId(int userId)
    {
        IEnumerable<FinancialAccountEntity> dbAccounts = await financialAccountRepository.GetAccountsAsync(userId);

        return dbAccounts.Select(a => a.ToDto());
    }
}