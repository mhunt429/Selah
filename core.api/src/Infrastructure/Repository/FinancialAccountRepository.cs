using Dapper;
using Microsoft.EntityFrameworkCore;
using Domain.Constants;
using Domain.Models.Entities.FinancialAccount;
using Infrastructure.Extensions;
using Infrastructure.Repository.Interfaces;

namespace Infrastructure.Repository;

public class FinancialAccountRepository(AppDbContext dbContext) : IFinancialAccountRepository
{
    public async Task ImportFinancialAccountsAsync(IEnumerable<FinancialAccountEntity> accounts)
    {
        await dbContext.FinancialAccounts.AddRangeAsync(accounts);
        await dbContext.SaveChangesAsync();
    }

    public async Task<int> AddAccountAsync(FinancialAccountEntity account)
    {
        await dbContext.FinancialAccounts.AddAsync(account);
        await dbContext.SaveChangesAsync();
        return account.Id;
    }

    public async Task<IEnumerable<FinancialAccountEntity?>> GetAccountsAsync(int userId)
    {
        return await dbContext.FinancialAccounts.Where(x => x.UserId == userId).ToListAsync();
    }

    public async Task<FinancialAccountEntity?> GetAccountByIdAsync(int userId, int id)
    {
        return await dbContext.FinancialAccounts
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
    }

    public async Task<bool> UpdateAccount(FinancialAccountEntity account)
    {
        var existing = dbContext.ChangeTracker.Entries<FinancialAccountEntity>()
            .FirstOrDefault(e => e.Entity.Id == account.Id);

        if (existing != null)
        {
            existing.State = EntityState.Detached;
        }

        dbContext.Attach(account);
        dbContext.Entry(account).State = EntityState.Modified;

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAccountAsync(FinancialAccountEntity account)
    {
        dbContext.Remove(account);
        await dbContext.SaveChangesAsync();
        return true;
    }
}