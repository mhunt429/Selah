using System.Threading.Channels;
using Domain.Events;
using Domain.MessageContracts;
using Infrastructure.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Channels;

public class PlaidWebhookProcessorChannel(
    ChannelReader<PlaidWebhookEvent> channelReader,
    ILogger<PlaidWebhookProcessorChannel> logger,
    ChannelWriter<ConnectorDataSyncEvent> channelWriter,
    IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await channelReader.WaitToReadAsync(stoppingToken))
        {
            while (channelReader.TryRead(out var item))
            {
                var @event = await MapWebHookEventToSyncEvent(item);

                if (@event != null)
                {
                    await channelWriter.WriteAsync(@event, stoppingToken);
                }
            }
        }
    }

    public async Task<ConnectorDataSyncEvent> MapWebHookEventToSyncEvent(PlaidWebhookEvent @event)
    {
        using var scope = scopeFactory.CreateScope();
        var connectorRepo = scope.ServiceProvider.GetRequiredService<IAccountConnectorRepository>();

        var connectorRecord = await connectorRepo.GetConnectorRecordByExternalId(@event.ItemId);
        if (connectorRecord == null) return null;

        EventType eventType = @event.PlaidWebhookType switch
        {
            PlaidWebhookType.SYNC_UPDATES_AVAILABLE => EventType.TransactionImport,
            PlaidWebhookType.RECURRING_TRANSACTION_UPDATE => EventType.RecurringTransactionImport
        };

        var retVal = new ConnectorDataSyncEvent
        {
            ConnectorId = connectorRecord.Id,
            EventType = eventType,
            UserId = connectorRecord.UserId,
            ItemId = @event.ItemId,
            AccessToken = connectorRecord.EncryptedAccessToken,
            TransactionSyncCursor = connectorRecord.TransactionSyncCursor
        };

        return retVal;
    }
}