using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities.Transactions;

[Table("transaction")]
public class TransactionEntity: BaseAuditFields
{
    [Column("id")] public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("transaction_date")]
    public DateTimeOffset TransactionDate { get; set; }

    [Column("imported_date")]
    public DateTimeOffset? ImportedDate { get; set; }

    [Column("pending")]
    public bool Pending { get; set; }

    [Column("account_id")]
    public int AccountId { get; set; }

    [Column("merchant_name")]
    public string MerchantName { get; set; } = string.Empty;

    [Column("transaction_name")]
    public string? TransactionName { get; set; }

    [Column("external_transaction_id")]
    public string? ExternalTransactionId { get; set; }

    [Column("merchant_logo_url")]
    public string? MerchantLogoUrl { get; set; }
}