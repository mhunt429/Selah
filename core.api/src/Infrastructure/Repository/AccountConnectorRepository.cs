using Domain.Models.Entities.AccountConnector;
using Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class AccountConnectorRepository(AppDbContext dbContext) : IAccountConnectorRepository
{
    /// <summary>
    ///     Insert into account_connector upon successful connection through Plaid or Finicity
    /// </summary>
    public async Task<int> InsertAccountConnectorRecord(AccountConnectorEntity account)
    {
        await dbContext.AddAsync(account);
        await dbContext.SaveChangesAsync();
        return account.Id;
    }

    public async Task<IEnumerable<AccountConnectorEntity>> GetAccountConnectorRecordsByUserId(int userId)
    {
        return await dbContext.AccountConnectors.Where(x => x.UserId == userId).ToListAsync();
    }


    public async Task UpdateConnectionSync(int id, int userId, DateTimeOffset nextDate)
    {
        await dbContext.AccountConnectors.Where(x => x.Id == id && x.UserId == userId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.LastSyncDate, DateTimeOffset.UtcNow)
                .SetProperty(x => x.NextSyncDate, nextDate)
            );
    }

    public async Task<IEnumerable<AccountConnectorEntity>> GetConnectorRecordsToImport()
    {
        return await dbContext.AccountConnectors.Where(x =>
            DateTimeOffset.UtcNow > x.NextSyncDate && !x.RequiresReauthentication
        ).ToListAsync();
    }

    public async Task<AccountConnectorEntity?> GetConnectorSyncRecordByConnectorId(int userId, int id)
    {
        return await dbContext.AccountConnectors
            .Where(x => x.Id == id && x.UserId == userId)
            .FirstOrDefaultAsync();
    }
    
    public async Task<int> LockRecordWhenAuthenticationIsRequired(int id, int userId)
    {
        return await dbContext.AccountConnectors.Where(x => x.Id == id && x.UserId == userId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.RequiresReauthentication, true)
            );
    }
}