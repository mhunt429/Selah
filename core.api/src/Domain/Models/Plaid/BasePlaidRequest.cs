using System.Text.Json.Serialization;

namespace Domain.Models.Plaid;

public class BasePlaidRequest
{
    [JsonPropertyName("client_id")] public required string ClientId { get; set; }

    [JsonPropertyName("secret")] public required string Secret { get; set; }

    [JsonPropertyName("access_token")] public string? AccessToken { get; set; }

    [JsonPropertyName("account_ids")] public List<string>? AccountIds { get; set; } = null;
}

