using System.Text.Json.Serialization;

namespace Domain.Models.Plaid;

public class PlaidTransactionsSyncRequest: BasePlaidRequest
{

    [JsonPropertyName("cursor")] public string? Cursor { get; set; }

    [JsonPropertyName("count")] public int? Count { get; set; }
}

