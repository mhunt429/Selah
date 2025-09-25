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
        var sql =
            @"INSERT INTO account_connector (
                               original_insert, 
                               last_update,
                               app_last_changed_by,
                               user_id, 
                               institution_id, 
                               institution_name, 
                               date_connected, 
                               encrypted_access_token, 
                               external_event_id) 
                            VALUES(
                                   @original_insert, 
                                   @last_update,
                                   @app_last_changed_by,
                                   @user_id, 
                                   @institution_id, 
                                   @institution_name, 
                                   @date_connected, 
                                   @encrypted_access_token, 
                                   @external_event_id)returning(id);";
        return await AddAsync<int>(sql, new
        {
            original_insert = DateTimeOffset.UtcNow,
            last_update = DateTimeOffset.UtcNow,
            app_last_changed_by = account.UserId,
            user_id = account.UserId,
            institution_id = account.InstitutionId,
            institution_name = account.InstitutionName,
            date_connected = DateTimeOffset.UtcNow,
            encrypted_access_token = account.EncryptedAccessToken,
            external_event_id = account.ExternalEventId
        });
    }

    public async Task<IEnumerable<AccountConnectorEntity>> GetAccountConnectorRecords(int userId)
    {
        var sql = "SELECT * FROM AccountConnector WHERE user_id = @userId";

        return await GetAllAsync<AccountConnectorEntity>(sql, new { userId });
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
}