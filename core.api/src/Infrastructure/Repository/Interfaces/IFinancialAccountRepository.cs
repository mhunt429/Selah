using Domain.Models.Entities.FinancialAccount;

namespace Infrastructure.Repository.Interfaces;

public interface IFinancialAccountRepository
{
    Task ImportFinancialAccountsAsync(IEnumerable<FinancialAccountEntity> accounts);

    Task<FinancialAccountEntity> AddAccountAsync(FinancialAccountEntity account);

    Task<IReadOnlyCollection<FinancialAccountEntity>> GetAccountsAsync(int userId);

    Task<FinancialAccountEntity?> GetAccountByIdAndUser(int id, int userId);

    Task<bool> UpdateAccount(FinancialAccountEntity account);

    Task<bool> DeleteAccountAsync(FinancialAccountEntity account);


    Task InsertBalanceHistory(AccountBalanceHistoryEntity history);

    Task<IReadOnlyCollection<FinancialAccountEntity?>> GetAccountsAsync(int userId, int connectorId);
    
}