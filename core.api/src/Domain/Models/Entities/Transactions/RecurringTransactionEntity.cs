using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models.Entities;

[Table("recurring_transaction")]
public class RecurringTransactionEntity : BaseAuditFields
{
    [Column("id")] public required int Id { get; set; }

    [Column("user_id")] public required int UserId { get; set; }
    [Column("account_id")] public required int AccountId { get; set; }
    [Column("external_id")] public required string ExternalId { get; set; }
    [Column("primary_description")] public required string PrimaryDescription { get; set; }
    [Column("detailed_description")] public required string DetailedDescription { get; set; }
    [Column("category_id")] public required int CategoryId { get; set; }
    [Column("average_amount")] public required decimal AverageAmount { get; set; }
    [Column("last_amount")] public required decimal LastAmount { get; set; }
    [Column("first_date")] public required DateTimeOffset FirstDate { get; set; }
    [Column("last_date")] public required DateTimeOffset LastDate { get; set; }
    [Column("predicted_next_date")] public required DateTimeOffset PredictedNextDate { get; set; }
    [Column("frequency")] public required string Frequency { get; set; }
}