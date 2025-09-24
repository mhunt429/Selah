namespace Domain.Models.AccountConnector;

public class ConnectionSyncData
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public ConnectionSyncType ConnectionSyncType { get; set; }

    public DateTimeOffset LastSyncDate { get; set; }

    public DateTimeOffset NextSyncDate { get; set; }

    public int ConnectorId { get; set; }
}

public enum ConnectionSyncType
{
    AccountBalance,
    Investments,
    Transactions,
    RecurringTransactions
}