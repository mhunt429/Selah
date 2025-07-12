using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities.ApplicationUser;

[Table("app_user")]
public class ApplicationUserEntity : BaseAuditFields
{
    [Key] [Column("id")] public Guid Id { get; set; }

    [Column("account_id")] public Guid AccountId { get; set; }

    [Column("created_date")] public DateTimeOffset CreatedDate { get; set; }

    [Column("encrypted_email")] public required string EncryptedEmail { get; set; }

    [Column("password")] public required string Password { get; set; }

    [Column("encrypted_name")] public required string EncryptedName { get; set; }

    [Column("encrypted_phone")] public required string EncryptedPhone { get; set; }

    [Column("last_login_date")] public DateTimeOffset? LastLoginDate { get; set; }

    [Column("last_login_ip")] public string LastLoginIp { get; set; } = string.Empty;

    [Column("phone_verified")] public bool PhoneVerified { get; set; }

    [Column("email_verified")] public bool EmailVerified { get; set; }

    [Column("email_hash")] public required string EmailHash { get; set; }
}