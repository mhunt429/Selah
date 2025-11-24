using Domain.ApiContracts.Banking;
using Domain.Models.Entities.FinancialAccount;
using Infrastructure.Repository.Interfaces;

namespace Application.Services;

public class BankingService
{
     private readonly IFinancialAccountRepository _financialAccountRepository;

     public BankingService(IFinancialAccountRepository financialAccountRepository)
     {
          _financialAccountRepository = financialAccountRepository;
     }

     public async Task<IEnumerable<FinancialAccountDto>> GetAccountsByUserId(int userId)
     {
          IEnumerable<FinancialAccountEntity> dbAccounts = await _financialAccountRepository.GetAccountsAsync(userId);

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