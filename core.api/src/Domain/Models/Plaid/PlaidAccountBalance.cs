using System.Text.Json.Serialization;

namespace Domain.Models.Plaid;

public class PlaidAccountBalance
{
    [JsonPropertyName("account_id")] public string AccountId { get; set; }

    [JsonPropertyName("balances")]
    public Balances? Balance { get; set; }

    [JsonPropertyName("mask")] public string Mask { get; set; }

    [JsonPropertyName("official_name")] public string OfficialName { get; set; }
    
    [JsonPropertyName("name")] public string Name { get; set; }
    
    [JsonPropertyName("subtype")] public string Subtype { get; set; }
}

public class Balances
{
    [JsonPropertyName("available")] public decimal Available { get; set; }

    [JsonPropertyName("current")] public decimal Current { get; set; }

    [JsonPropertyName("iso_currency_code")]
    public string IsoCurrencyCode { get; set; }
}