using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities.AccountConnector;

[Table("account_connector")]
public class AccountConnectorEntity : BaseAuditFields
{
    [Key, Column(name: "id", Order = 0)]
    public int Id { get; set; }

    [Column("user_id")] public int UserId { get; set; }

    [Column("institution_id")] public required string InstitutionId { get; set; }

    [Column("institution_name")] public required string InstitutionName { get; set; }

    [Column("date_connected")] public required DateTimeOffset DateConnected { get; set; }

    [Column("encrypted_access_token")] public required string EncryptedAccessToken { get; set; }

    [Column("transaction_sync_cursor")] public required string TransactionSyncCursor { get; set; } = "";

    [Column("external_event_id")]
    //Set this field for the 3rd party webhooks
    public string ExternalEventId { get; set; } = "";
}