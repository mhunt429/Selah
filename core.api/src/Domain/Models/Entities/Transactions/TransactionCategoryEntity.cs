using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities.Transactions;

[Table("user_transaction_category")]
public class TransactionCategoryEntity: BaseAuditFields
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("user_id")]
    public required int UserId { get; set; }
    
    [Column("category_name")]
    public required string CategoryName { get; set; }
}