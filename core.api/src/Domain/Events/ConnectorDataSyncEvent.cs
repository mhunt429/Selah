namespace Domain.Events;

public class ConnectorDataSyncEvent : IntegrationEvent
{
    public byte[] AccessToken { get; set; }

    public int UserId { get; set; }
}