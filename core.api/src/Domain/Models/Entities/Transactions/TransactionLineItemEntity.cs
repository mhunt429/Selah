using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities.Transactions;

[Table("transaction_line_item")]
public class TransactionLineItemEntity: BaseAuditFields
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("transaction_id")]
    public int TransactionId { get; set; }
    
    [Column("description")]
    public string? Description { get; set; }    
    
    [Column("amount")]
    public decimal Amount { get; set; }
    
    [Column("category_id")]
    public int CategoryId { get; set; }

    public virtual TransactionCategoryEntity Category { get; set; } = null!;
    
    public virtual TransactionEntity Transaction { get; set; } = null!;
}