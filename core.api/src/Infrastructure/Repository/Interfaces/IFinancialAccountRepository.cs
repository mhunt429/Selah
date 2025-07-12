using Domain.Models.Entities.FinancialAccount;

namespace Infrastructure.Repository.Interfaces;

public interface IFinancialAccountRepository
{
    Task ImportFinancialAccountsAsync(IEnumerable<FinancialAccountEntity> accounts);

    Task<Guid> AddAccountAsync(FinancialAccountEntity account);

    Task<IEnumerable<FinancialAccountEntity?>> GetAccountsAsync(Guid userId);

    Task<FinancialAccountEntity?> GetAccountByIdAsync(Guid userId, Guid id);

    Task<bool> UpdateAccount(FinancialAccountEntity account);

    Task<bool> DeleteAccountAsync(FinancialAccountEntity account);
}