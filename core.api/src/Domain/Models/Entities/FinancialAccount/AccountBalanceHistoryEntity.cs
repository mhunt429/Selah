using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities.FinancialAccount;

[Table("account_balance_history")]
public class AccountBalanceHistoryEntity
{
    [Column("id")] public int Id { get; set; }

    [Column("user_id")] public int UserId { get; set; }

    [Column("account_id")] public int FinancialAccountId { get; set; }

    [Column("current_balance")] public decimal CurrentBalance { get; set; }

    [Column("created_at")] public DateTimeOffset CreatedAt { get; set; }
}