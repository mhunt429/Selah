using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities.Transactions;

[Table("transaction_line_item")]
public class TransactionLineItemEntity: BaseAuditFields
{
    public int Id { get; set; }
    
    public int TransactionId { get; set; }
    
    public string? Description { get; set; }    
    
    public decimal Amount { get; set; }
    
    public int CategoryId { get; set; }
}