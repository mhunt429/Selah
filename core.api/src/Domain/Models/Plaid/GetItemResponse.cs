using System.Text.Json.Serialization;

namespace Domain.Models.Plaid;

public class GetItemResponse
{
    [JsonPropertyName("item")]
    public required Item Item { get; set; }
}

public class Item
{
    [JsonPropertyName("error")]
    public PlaidApiErrorResponse? Error { get; set; }
}