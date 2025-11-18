using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities.Identity;

[Table("user_session")]
public class UserSessionEntity : BaseAuditFields
{
    [Column("id")] public required Guid Id { get; set; }

    [Column("user_id")] public required int UserId { get; set; }

    [Column("issued_at")] public required DateTimeOffset IssuedAt { get; set; }

    [Column("expires_at")] public required DateTimeOffset ExpiresAt { get; set; }
}