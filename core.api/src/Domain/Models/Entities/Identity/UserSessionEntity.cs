using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities.Identity;

[Table("user_session")]
public class UserSessionEntity : BaseAuditFields
{
    [Column("id")] public Guid Id { get; set; }

    [Column("user_id")] public Guid UserId { get; set; }

    [Column("session_id")] public Guid SessionId { get; set; }

    [Column("issued_at")] public DateTimeOffset IssuedAt { get; set; }

    [Column("expires_at")] public DateTimeOffset ExpiresAt { get; set; }
}