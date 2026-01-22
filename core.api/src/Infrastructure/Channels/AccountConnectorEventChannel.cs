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
using Microsoft.Extensions.Logging;

namespace Infrastructure.Channels;

public class AccountConnectorEventChannel : BackgroundService
{
    private readonly ChannelReader<ConnectorDataSyncEvent> _reader;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AccountConnectorEventChannel> _logger;

    public AccountConnectorEventChannel(
        ChannelReader<ConnectorDataSyncEvent> reader,
        IServiceScopeFactory scopeFactory, ILogger<AccountConnectorEventChannel> logger)
    {
        _reader = reader;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _reader.WaitToReadAsync(stoppingToken))
        {
            while (_reader.TryRead(out ConnectorDataSyncEvent? @event))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var connectorEventService = scope.ServiceProvider.GetRequiredService<IConnectorEventService>();

                    await connectorEventService.ProcessEventAsync(@event);
                }

                catch (Exception ex)
                {
                    _logger.LogError("Event Processing for event type {@EventType} for user {UserId} failed with exception: {@Exception}", @event.EventType, @event.UserId, ex.Message + ex.StackTrace);
                    continue;
                }
            }
        }
    }
}