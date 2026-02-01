using System.Text.Json.Serialization;

namespace Domain.ApiContracts.Connector;

public class PlaidWebhookRequest
{
    [JsonPropertyName("webhook_code")]
    public required string WebhookCode { get; set; }
 
    [JsonPropertyName("item_id")]
    public required string ItemId { get; set; }
    
    [JsonPropertyName("error")]
    public WebhookError? Error { get; set; }
}

public class WebhookError
{
    [JsonPropertyName("error_message")]
    public required string ErrorMessage { get; set; }
}