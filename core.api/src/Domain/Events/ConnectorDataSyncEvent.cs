using Domain.Models.Plaid;

namespace Domain.Events;

public class ConnectorDataSyncEvent : IntegrationEvent
{
    public required byte[] AccessToken { get; set; }
    
    public required int ConnectorId { get; set; }
    
    public int UserId { get; set; }
    
    public string? ItemId { get; set; }
    
    public required EventType EventType { get; set; }
    
    public PlaidApiErrorResponse?  Error { get; set; }
    
    public string? TransactionSyncCursor { get; set; }
    
}


public enum EventType
{
    BalanceImport,
    TransactionImport,
    UpdateCredentials,
    RecurringTransactionImport
}