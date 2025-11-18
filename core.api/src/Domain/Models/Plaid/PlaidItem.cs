using System.Text.Json.Serialization;

namespace Domain.Models.Plaid;

public class PlaidItem
{
    [JsonPropertyName("institution_id")] public required string InstitutionId { get; set; }

    [JsonPropertyName("institution_name")] public required string InstitutionName { get; set; }

    [JsonPropertyName("item_id")] public required string ItemId { get; set; }
}