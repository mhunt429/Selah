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

    public async Task<Guid> AddAccountAsync(FinancialAccountEntity account)
    {
        account.Id = Guid.CreateVersion7(DateTime.UtcNow);
        await dbContext.FinancialAccounts.AddAsync(account);
        await dbContext.SaveChangesAsync();
        return account.Id;
    }

    public async Task<IEnumerable<FinancialAccountEntity?>> GetAccountsAsync(Guid userId)
    {
        return await dbContext.FinancialAccounts.Where(x => x.UserId == userId).ToListAsync();
    }

    public async Task<FinancialAccountEntity?> GetAccountByIdAsync(Guid userId, Guid id)
    {
        return await dbContext.FinancialAccounts
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
    }

    public async Task<bool> UpdateAccount(FinancialAccountEntity account)
    {
        dbContext.FinancialAccounts.Update(account);
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAccountAsync(FinancialAccountEntity account)
    {
        dbContext.Remove(account);
        return await dbContext.SaveChangesAsync() > 0;
    }
}