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
    Task<IEnumerable<AccountConnectorEntity>> GetAccountConnectorRecords(int userId);

    /// <summary>
    /// Updates the specific connector record after the recurring job as has ran
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <param name="nextDate"></param>
    /// <returns></returns>
    Task UpdateConnectionSync(int id, int userId, DateTimeOffset nextDate);

    Task<IEnumerable<ConnectionSyncDataEntity>> GetConnectorRecordsToImport();

    Task<ConnectionSyncDataEntity> GetConnectorSyncRecordByConnectorId(int userId, int connectorId);
}