using System.Text.Json.Serialization;

namespace Domain.Models.Plaid;

public class PlaidWebhookVerificationResponse
{
    [JsonPropertyName("key")]
    public WebhookKey? Key { get; set; }
}

public class WebhookKey
{
    [JsonPropertyName("alg")]
    public string? Alg { get; set; }
    
    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; }
    
    [JsonPropertyName("crv")]
    public string? Crv { get; set; }
    
    [JsonPropertyName("expired_at")]
    public long ExpiredAt { get; set; }
    
    
    [JsonPropertyName("kid")]
    public string? Kid { get; set; }
}

public class WebhookVerificationRequest : BasePlaidRequest
{
    [JsonPropertyName("key_id")]
    public  required string KeyId { get; set; }
}