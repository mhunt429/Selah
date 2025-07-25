using System.Text.Json.Serialization;

namespace Domain.Models.Plaid;

public class PlaidBalanceApiResponse
{
    [JsonPropertyName("accounts")] public required IEnumerable<PlaidAccountBalance> Accounts { get; set; }

    [JsonPropertyName("item")] public PlaidItem? Item { get; set; }
}