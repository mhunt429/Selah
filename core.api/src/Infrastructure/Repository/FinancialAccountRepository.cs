using Domain.Models.Entities.FinancialAccount;
using Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class FinancialAccountRepository(AppDbContext dbContext) : IFinancialAccountRepository
{

    public async Task ImportFinancialAccountsAsync(IEnumerable<FinancialAccountEntity> accounts)
    {
        await dbContext.FinancialAccounts.AddRangeAsync(accounts);
        await dbContext.SaveChangesAsync();
    }

    public async Task<FinancialAccountEntity> AddAccountAsync(FinancialAccountEntity account)
    {
        await dbContext.FinancialAccounts.AddAsync(account);
        await dbContext.SaveChangesAsync();
        return account;
    }

    public async Task InsertBalanceHistory(AccountBalanceHistoryEntity history)
    {
        await dbContext.AccountBalanceHistory.AddAsync(history);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<FinancialAccountEntity>> GetAccountsAsync(int userId)
    {
        return await dbContext.FinancialAccounts
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<FinancialAccountEntity?>> GetAccountsAsync(int userId, int connectorId)
    {
        return await dbContext.FinancialAccounts
            .AsNoTracking()
            .Where(a => a.UserId == userId && a.ConnectorId == connectorId)
            .ToListAsync();
    }

    public async Task<FinancialAccountEntity?> GetAccountByIdAndUser(int id, int userId)
    {
        return await dbContext.FinancialAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
    }

    public async Task<IEnumerable<AccountBalanceHistoryEntity>> GetBalanceHistory(
        int userId,
        int accountId)
    {
        return await dbContext.AccountBalanceHistory
            .AsNoTracking()
            .Where(h => h.UserId == userId && h.FinancialAccountId == accountId)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();
    }


    public async Task<bool> UpdateAccount(FinancialAccountEntity account)
    {
        var rows = await dbContext.FinancialAccounts
            .Where(a => a.Id == account.Id && a.UserId == account.UserId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(a => a.CurrentBalance, account.CurrentBalance)
                .SetProperty(a => a.DisplayName, account.DisplayName)
                .SetProperty(a => a.OfficialName, account.OfficialName)
                .SetProperty(a => a.Subtype, account.Subtype)
                .SetProperty(a => a.LastApiSyncTime, account.LastApiSyncTime)
                .SetProperty(a => a.AppLastChangedBy, account.UserId)
            );

        return rows > 0;
    }

    public async Task<bool> DeleteAccountAsync(FinancialAccountEntity account)
    {
        var rows = await dbContext.FinancialAccounts
            .Where(a => a.Id == account.Id && a.UserId == account.UserId)
            .ExecuteDeleteAsync();

        return rows > 0;
    }
}
