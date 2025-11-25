namespace Domain.Events;

public class ConnectorDataSyncEvent : IntegrationEvent
{
    public byte[] AccessToken { get; set; }

    public required int DataSyncId { get; set; }
    
    public required int ConnectorId { get; set; }
    
    public int UserId { get; set; }
}