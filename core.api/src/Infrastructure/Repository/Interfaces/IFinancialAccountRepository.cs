using Domain.Models.Entities.FinancialAccount;

namespace Infrastructure.Repository.Interfaces;

public interface IFinancialAccountRepository
{
    Task ImportFinancialAccountsAsync(IEnumerable<FinancialAccountEntity> accounts);

    Task<FinancialAccountEntity> AddAccountAsync(FinancialAccountEntity account);

    Task<IEnumerable<FinancialAccountEntity>> GetAccountsAsync(int userId);

    Task<FinancialAccountEntity?> GetAccountByIdAsync(int userId, int id);

    Task<bool> UpdateAccount(FinancialAccountEntity account);

    Task<bool> DeleteAccountAsync(FinancialAccountEntity account);


    Task InsertBalanceHistory(AccountBalanceHistoryEntity history);

    Task<IEnumerable<AccountBalanceHistoryEntity>> GetBalanceHistory(int userId, int accountId);

    Task<IEnumerable<FinancialAccountEntity?>> GetAccountsAsync(int userId, int connectorId);
    
}