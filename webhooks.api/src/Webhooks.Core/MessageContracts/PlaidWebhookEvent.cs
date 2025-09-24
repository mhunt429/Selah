using Webhooks.Core.Enums;

namespace Webhooks.Core.MessageContracts;

public class PlaidWebhookEvent
{
    public required Guid EventId { get; set; }
    
    public required string ItemId { get; set; }
    
    public DateTimeOffset DateSent { get; set; }
    
    public  PlaidWebhookType PlaidWebhookType { get; set; }
}