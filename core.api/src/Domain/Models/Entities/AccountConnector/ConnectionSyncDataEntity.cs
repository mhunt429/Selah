using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities.AccountConnector;

[Table("connection_sync_data")]
public class ConnectionSyncDataEntity : BaseAuditFields
{
    [Key] [Column("id")] public int Id { get; set; }

    [Column("user_id")] public int UserId { get; set; }

    [Column("last_sync_date")] public DateTimeOffset LastSyncDate { get; set; }

    [Column("next_sync_date")] public DateTimeOffset NextSyncDate { get; set; }

    [Column("connector_id")] public int ConnectorId { get; set; }
    
    [Column("encrypted_access_token")] public required byte[] EncryptedAccessToken { get; set; }
}