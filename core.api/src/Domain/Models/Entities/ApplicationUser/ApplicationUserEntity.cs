using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models.Entities.UserAccount;

namespace Domain.Models.Entities.ApplicationUser;

[Table("app_user")]
public class ApplicationUserEntity : BaseAuditFields
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("account_id")] public int AccountId { get; set; }
    
    [ForeignKey("AccountId")]
    public virtual required UserAccountEntity UserAccount { get; set; }

    [Column("created_date")] public DateTimeOffset CreatedDate { get; set; }

    [Column("encrypted_email")] public required byte[] EncryptedEmail { get; set; }

    [Column("password")] public required string Password { get; set; }

    [Column("encrypted_name")] public required byte[] EncryptedName { get; set; }

    [Column("encrypted_phone")] public required byte[] EncryptedPhone { get; set; }

    [Column("last_login_date")] public DateTimeOffset? LastLoginDate { get; set; }

    [Column("last_login_ip")] public string LastLoginIp { get; set; } = string.Empty;

    [Column("phone_verified")] public bool PhoneVerified { get; set; }

    [Column("email_verified")] public bool EmailVerified { get; set; }

    [Column("email_hash")] public required string EmailHash { get; set; }

}