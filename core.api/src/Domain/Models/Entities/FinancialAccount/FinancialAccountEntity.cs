using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities.FinancialAccount;

[Table("financial_account")]
public class FinancialAccountEntity : BaseAuditFields
{
   
    [Key, Column(name: "id", Order = 0)]
    public Guid Id { get; set; }

    [Column("connector_id")] public Guid ConnectorId { get; set; }

    [Column("user_id")] public Guid UserId { get; set; }

    [Column("external_id")] public string ExternalId { get; set; } = "";

    [Column("current_balance")] public decimal CurrentBalance { get; set; }

    [Column("account_mask")] public string AccountMask { get; set; } = "";

    [Column("display_name")] public required string DisplayName { get; set; }

    [Column("official_name")] public string OfficialName { get; set; } = "";

    [Column("subtype")] public required string Subtype { get; set; }

    [Column("is_external_api_import")] public bool IsExternalApiImport { get; set; }

    
    [Column("last_api_sync_time")] public DateTimeOffset LastApiSyncTime { get; set; }
}