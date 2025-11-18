namespace Domain.MessageContracts;

public class PlaidWebhookEvent
{
    public required Guid EventId { get; set; }

    public required string ItemId { get; set; }

    public DateTimeOffset DateSent { get; set; }

    public PlaidWebhookType PlaidWebhookType { get; set; }
}

public enum PlaidWebhookType
{
    SYNC_UPDATES_AVAILABLE,
    DEFAULT_UPDATE,
    INITIAL_UPDATE,
    HISTORICAL_UPDATE,
    RECURRING_TRANSACTION_UPDATE
}