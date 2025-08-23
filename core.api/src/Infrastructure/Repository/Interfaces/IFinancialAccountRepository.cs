using Domain.Models.Entities.FinancialAccount;

namespace Infrastructure.Repository.Interfaces;

public interface IFinancialAccountRepository
{
    Task ImportFinancialAccountsAsync(IEnumerable<FinancialAccountEntity> accounts);

    Task<int> AddAccountAsync(FinancialAccountEntity account);

    Task<IEnumerable<FinancialAccountEntity?>> GetAccountsAsync(int userId);

    Task<FinancialAccountEntity?> GetAccountByIdAsync(int userId, int id);

    Task<bool> UpdateAccount(FinancialAccountUpdate account, int id, int userId);

    Task<bool> DeleteAccountAsync(FinancialAccountEntity account);

    Task InsertBalanceHistory(AccountBalanceHistoryEntity history, int userId);
    Task<IEnumerable<AccountBalanceHistoryEntity>> GetBalanceHistory(int userId, int accountId);
}