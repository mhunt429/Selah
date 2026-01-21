using System.Threading.Channels;
using Domain.Events;
using Domain.Models.Entities.AccountConnector;
using Domain.Models.Entities.Mailbox;
using Domain.Models.Plaid;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Connector;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Channels;

public class AccountConnectorEventChannel : BackgroundService
{
    private readonly ChannelReader<ConnectorDataSyncEvent> _reader;
    private readonly IServiceScopeFactory _scopeFactory;

    public AccountConnectorEventChannel(
        ChannelReader<ConnectorDataSyncEvent> reader,
        IServiceScopeFactory scopeFactory)
    {
        _reader = reader;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _reader.WaitToReadAsync(stoppingToken))
        {
            while (_reader.TryRead(out ConnectorDataSyncEvent? @event))
            {
               using var scope = _scopeFactory.CreateScope();
               var connectorEventService = scope.ServiceProvider.GetRequiredService<IConnectorEventService>();
               
               await connectorEventService.ProcessEventAsync(@event);
            }
        }
    }
}