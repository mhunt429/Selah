namespace Domain.Configuration;

public class SecurityConfig
{
    public required string JwtSecret { get; set; }

    public required string CryptoSecret { get; set; }

    public required int AccessTokenExpiryMinutes { get; set; }

    public required int RefreshTokenExpiryDays { get; set; }
}