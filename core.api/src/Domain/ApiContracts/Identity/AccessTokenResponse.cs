namespace Domain.ApiContracts.Identity;

public class AccessTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public long AccessTokenExpiration { get; set; }

    public long RefreshTokenExpiration { get; set; }

    public Guid SessionId { get; set; }

    public DateTimeOffset SessionExpiration { get; set; }
}