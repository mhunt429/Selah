using Domain.Models.Entities.AccountConnector;

namespace Infrastructure.Repository.Interfaces;

public interface IAccountConnectorRepository
{
    /// <summary>
    /// Saves the intial connector record when a user connects their institution
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    Task<int> InsertAccountConnectorRecord(AccountConnectorEntity account);

    /// <summary>
    /// Gets all account connectors for a specific user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<IEnumerable<AccountConnectorEntity>> GetAccountConnectorRecordsByUserId(int userId);

    /// <summary>
    /// Updates the transactions cursor after a given sync
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <param name="nextCursor"></param>
    /// <returns></returns>
    Task UpdateConnectionSyncCursor(int id, int userId, string? nextCursor);

    Task<IEnumerable<AccountConnectorEntity>> GetConnectorRecordsToImport();


    Task<AccountConnectorEntity?> GetConnectorRecordByIdAndUser(int id, int userId);


    Task<int> LockRecordWhenAuthenticationIsRequired(int id, int userId);

    Task<AccountConnectorEntity?> GetConnectorRecordByExternalId(string externalId);

    Task UpdateAccountSyncTimes(int id, int userId, DateTimeOffset nextSyncDate);

    /// <summary>
    /// When a user re-authenticates, we want to remove the lock on the connector record
    /// so that we can begin importing data again
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> RemoveConnectionSyncLock(int id, int userId);
}