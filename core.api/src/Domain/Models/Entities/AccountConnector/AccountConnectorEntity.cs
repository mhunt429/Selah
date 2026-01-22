using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities.AccountConnector;

[Table("account_connector")]
public class AccountConnectorEntity : BaseAuditFields
{
    [Key, Column(name: "id", Order = 0)] public int Id { get; set; }

    [Column("user_id")] public int UserId { get; set; }

    [Column("institution_id")] public required string InstitutionId { get; set; }

    [Column("institution_name")] public required string InstitutionName { get; set; }

    [Column("date_connected")] public required DateTimeOffset DateConnected { get; set; }

    [Column("encrypted_access_token")] public required byte[] EncryptedAccessToken { get; set; }

    [Column("transaction_sync_cursor")] public required string? TransactionSyncCursor { get; set; } = "";

    [Column("requires_reauthentication")] public bool RequiresReauthentication { get; set; }

    [Column("external_event_id")]
    //Set this field for the 3rd party webhooks
    public string ExternalEventId { get; set; } = "";

    [Column("last_sync_date")] public DateTimeOffset LastSyncDate { get; set; }

    [Column("next_sync_date")] public DateTimeOffset NextSyncDate { get; set; }
}