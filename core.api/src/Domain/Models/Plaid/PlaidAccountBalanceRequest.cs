using System.Text.Json.Serialization;

namespace Domain.Models.Plaid;

public class PlaidAccountBalanceRequest
{
    [JsonPropertyName("client_id")] public required string ClientId { get; set; }

    [JsonPropertyName("secret")] public required string Secret { get; set; }

    [JsonPropertyName("access_token")] public required string AccessToken { get; set; }
}