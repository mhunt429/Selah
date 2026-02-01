using System.Text.Json.Serialization;

namespace Domain.Models.Plaid;

public class PlaidAccountBalance
{
    [JsonPropertyName("account_id")] public required string AccountId { get; set; }

    [JsonPropertyName("balances")]
    public Balances? Balance { get; set; }

    [JsonPropertyName("mask")] public required string Mask { get; set; }

    [JsonPropertyName("official_name")] public required string OfficialName { get; set; }
    
    [JsonPropertyName("name")] public required string Name { get; set; }
    
    [JsonPropertyName("subtype")] public required string Subtype { get; set; }
}

public class Balances
{
    [JsonPropertyName("available")] public decimal Available { get; set; }

    [JsonPropertyName("current")] public decimal Current { get; set; }
}