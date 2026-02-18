namespace Domain.ApiContracts.Connector;

public class ConnectionRefreshResult
{
    public required int Id { get; set; }

    public required bool Success { get; set; }
}