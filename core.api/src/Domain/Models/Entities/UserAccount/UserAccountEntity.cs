using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities.UserAccount;

[Table("user_account")]
public class UserAccountEntity : BaseAuditFields
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("created_on")] public DateTimeOffset CreatedOn { get; set; }
    [Column("account_name")] public string? AccountName { get; set; }
}