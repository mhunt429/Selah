using System.Text.Json.Serialization;

namespace Domain.ApiContracts.Identity;

public class AccessTokenResponse
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;

    [JsonPropertyName("accessTokenExpiration")]
    public long AccessTokenExpiration { get; set; }

    [JsonPropertyName("refreshTokenExpiration")]
    public long RefreshTokenExpiration { get; set; }

    [JsonPropertyName("sessionId")]
    public Guid SessionId { get; set; }

    [JsonPropertyName("sessionExpiration")]
    public DateTimeOffset SessionExpiration { get; set; }
}