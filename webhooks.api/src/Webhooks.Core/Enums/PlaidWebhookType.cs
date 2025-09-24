namespace Webhooks.Core.Enums;

public enum PlaidWebhookType
{
    SYNC_UPDATES_AVAILABLE,
    DEFAULT_UPDATE,
    INITIAL_UPDATE,
    HISTORICAL_UPDATE,
    RECURRING_TRANSACTION_UPDATE
}