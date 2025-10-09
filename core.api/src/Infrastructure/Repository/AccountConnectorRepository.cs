using Domain.Models.Entities.AccountConnector;
using Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class AccountConnectorRepository(IDbConnectionFactory dbConnectionFactory)
    : BaseRepository(dbConnectionFactory), IAccountConnectorRepository
{
    /// <summary>
    ///     Insert into account_connector upon successful connection through Plaid or Finicity
    /// </summary>
    public async Task<int> InsertAccountConnectorRecord(AccountConnectorEntity account)
    {
        var connectorSql =
            @"INSERT INTO account_connector (
                               app_last_changed_by,
                               user_id, 
                               institution_id, 
                               institution_name, 
                               date_connected, 
                               encrypted_access_token, 
                               external_event_id) 
                            VALUES(
                                   @app_last_changed_by,
                                   @user_id, 
                                   @institution_id, 
                                   @institution_name, 
                                   @date_connected, 
                                   @encrypted_access_token, 
                                   @external_event_id)returning(id);";

        var connectorDataToSave = new
        {
            app_last_changed_by = account.UserId,
            user_id = account.UserId,
            institution_id = account.InstitutionId,
            institution_name = account.InstitutionName,
            date_connected = DateTimeOffset.UtcNow,
            encrypted_access_token = account.EncryptedAccessToken,
            external_event_id = account.ExternalEventId
        };
        
        int accountConnectorId = await AddAsync<int>(connectorSql,connectorDataToSave);

        var connectionSyncSql =
            @"INSERT INTO connection_sync_data(user_id, last_sync_date, next_sync_date, connector_id, app_last_changed_by)
                VALUES(@userId, CURRENT_TIMESTAMP, @nextSyncDate, @connectorId, @appLastChangedBy)";

        var connectorSyncDataToSave = new
        {
            userId = account.UserId,
            nextSyncDate = DateTimeOffset.UtcNow.AddDays(3),
            connectorId = accountConnectorId,
            appLastChangedBy = account.UserId
        };
        
        await AddAsync<int>(connectionSyncSql,connectorSyncDataToSave);
        
        return accountConnectorId;
    }

    public async Task<IEnumerable<AccountConnectorEntity>> GetAccountConnectorRecordsByUserId(int userId)
    {
        var sql = "SELECT * FROM AccountConnector WHERE user_id = @userId";

        return await GetAllAsync<AccountConnectorEntity>(sql, new { userId });
    }

    public async Task<AccountConnectorEntity> GetAccountConnectorRecordById(int id)
    {
        var sql = "SELECT * FROM AccountConnector WHERE id = @id";
        return await GetFirstOrDefaultAsync<AccountConnectorEntity>(sql, new { id });
    }

    public async Task UpdateConnectionSync(int id, int userId, DateTimeOffset nextDate)
    {
        string sql = @"UPDATE connection_sync_data 
            SET 
                last_sync_date = @lastSyncDate, 
                next_sync_date = @nextSyncDate WHERE id = @id AND user_id = @userId";

        await UpdateAsync(sql,
            new
            {
                id = id, userId = userId, @lastSyncDate = DateTimeOffset.UtcNow, @nextSyncDate = nextDate
            });
    }

    public async Task<IEnumerable<ConnectionSyncDataEntity>> GetConnectorRecordsToImport()
    {
        var sql = "SELECT * FROM connection_sync_data WHERE CURRENT_TIMESTAMP > next_sync_date";

        return await GetAllAsync<ConnectionSyncDataEntity>(sql, null);
    }
    
    public async Task<ConnectionSyncDataEntity> GetConnectorSyncRecordByConnectorId(int userId, int connectorId)
    {
        var sql = "SELECT * FROM connection_sync_data WHERE connector_id = @connectorId AND user_id = @userId";

        return await GetFirstOrDefaultAsync<ConnectionSyncDataEntity>(sql, new { connectorId, userId });
    }
}