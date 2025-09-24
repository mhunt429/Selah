namespace Domain.ApiContracts.Connector;

public class ConnectionSyncDataApiResponse
{
    public int ConnectorId { get; set; }

    public DateTimeOffset LastSyncDate { get; set; }

    public DateTimeOffset NextSyncDate { get; set; }

    public required ConnectorApiResponse ConnectorMetadata { get; set; }
}