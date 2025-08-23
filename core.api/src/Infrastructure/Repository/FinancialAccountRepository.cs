using Domain.Models.Entities.FinancialAccount;
using Infrastructure.Repository.Interfaces;

namespace Infrastructure.Repository;

public class FinancialAccountRepository : BaseRepository, IFinancialAccountRepository
{
    private readonly AppDbContext _dbContext;

    public FinancialAccountRepository(AppDbContext dbContext, IDbConnectionFactory dbConnectionFactory) : base(
        dbConnectionFactory)
    {
        _dbContext = dbContext;
    }

    public async Task ImportFinancialAccountsAsync(IEnumerable<FinancialAccountEntity> accounts)
    {
        await _dbContext.FinancialAccounts.AddRangeAsync(accounts);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<int> AddAccountAsync(FinancialAccountEntity account)
    {
        await _dbContext.FinancialAccounts.AddAsync(account);
        await _dbContext.SaveChangesAsync();
        return account.Id;
    }

    public async Task<IEnumerable<FinancialAccountEntity?>> GetAccountsAsync(int userId)
    {
        string sql = "SELECT * FROM financial_account WHERE user_id = @userId";
       return await GetAllAsync<FinancialAccountEntity>(sql, new { userId });
    }

    public async Task<FinancialAccountEntity?> GetAccountByIdAsync(int userId, int id)
    {
        string sql = "SELECT * FROM financial_account WHERE id = @id AND user_id = @userId";
        return await GetFirstOrDefaultAsync<FinancialAccountEntity>(sql, new { id, userId });
    }
    
    public async Task<bool> UpdateAccount(FinancialAccountUpdate account, int id, int userId)
    {
        string sql = @"UPDATE financial_account SET current_balance = @currentBalance, 
            display_name = @displayName, 
            official_name = @officialName, 
            subtype = @subtype,
            last_api_sync_time = @lastApiSyncTime,
            app_last_changed_by = @appLastChangedBy,
            last_update = @lastUpdate
            WHERE id = @id AND user_id = @userId";

        var modelToSave = new
        {
            id,
            userId,
            displayName = account.DisplayName,
            officialName = account.OfficialName,
            subtype = account.Subtype,
            lastApiSyncTime = account.LastApiSyncTime,
            appLastChangedBy = userId,
            lastUpdate = DateTimeOffset.UtcNow,
            currentBalance = account.CurrentBalance,
        };

        return await UpdateAsync(sql, modelToSave);
    }

    public async Task<bool> DeleteAccountAsync(FinancialAccountEntity account)
    {
       return await DeleteAsync("DELETE FROM financial_account WHERE id = @id AND user_id = @userId", new { account.Id, account.UserId });
    }
}