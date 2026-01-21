using Domain.Events;

namespace Infrastructure.Services.Interfaces;

public interface IConnectorEventService
{
    Task ProcessEventAsync(ConnectorDataSyncEvent @event);
}