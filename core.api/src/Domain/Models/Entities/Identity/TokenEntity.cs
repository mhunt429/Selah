using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities.Identity;

[Table("token")]
public class TokenEntity : BaseAuditFields
{
    [Column("id")] public int Id { get; set; }

    [Column("user_id")] public required int UserId { get; set; }

    [Column("token")] public required string Token { get; set; }

    [Column("token_type")] public required string TokenType { get; set; }

    [Column("expires_at")] public required DateTimeOffset ExpiresAt { get; set; }

    [Column("created_at")] public required DateTimeOffset CreatedAt { get; set; }
}

public static class TokenType
{
    public const string AccessToken = "access_token";

    public const string RefreshToken = "refresh_token";
}