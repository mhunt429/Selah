using System.Threading.Channels;
using Domain.Events;
using Infrastructure.Services.Connector;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Channels;

public class AccountBalanceImportChannel : BackgroundService
{
    private readonly ChannelReader<ConnectorDataSyncEvent> _reader;
    private readonly IServiceScopeFactory _scopeFactory;

    public AccountBalanceImportChannel(
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
            while (_reader.TryRead(out ConnectorDataSyncEvent? connectorDataSyncEvent))
            {
                using var scope = _scopeFactory.CreateScope();

                var importService = scope.ServiceProvider
                    .GetRequiredService<PlaidAccountBalanceImportService>();

                await importService.ImportAccountBalancesAsync(connectorDataSyncEvent);
            }
        }
    }
}