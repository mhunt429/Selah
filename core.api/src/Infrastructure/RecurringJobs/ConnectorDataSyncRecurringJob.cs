using System.Threading.Channels;
using Domain.Events;
using Domain.Extensions;
using Microsoft.Extensions.Logging;
using Quartz;
using Domain.Models.Entities.AccountConnector;
using Infrastructure.Repository.Interfaces;

namespace Infrastructure.RecurringJobs;

public class ConnectorDataSyncRecurringJob(
    ILogger<ConnectorDataSyncRecurringJob> logger,
    IAccountConnectorRepository connectorRepository,
    ChannelWriter<ConnectorDataSyncEvent> channelWriter)
    : IJob
{

    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("ConnectorDataSyncRecurringJob started at {CurrentTimeUtc}", DateTimeOffset.UtcNow);

        IEnumerable<AccountConnectorEntity> dbRecords = await connectorRepository.GetConnectorRecordsToImport();

        logger.LogInformation("ConnectorDataSyncRecurringJob found {numRecords} connection records to import",
            dbRecords.Count());

        var groups = dbRecords.GroupByCount(10);
        foreach (var group in groups)
        {
            var publisherTasks = new List<Task>();
            foreach (AccountConnectorEntity connectorRecord in group)
            {
                publisherTasks.Add(PublishSyncEvent(new ConnectorDataSyncEvent
                {
                    AccessToken = connectorRecord.EncryptedAccessToken, 
                    UserId =  connectorRecord.UserId,
                    ConnectorId = connectorRecord.Id,
                    EventType = EventType.BalanceImport,
                }));
            }
            
            await Task.WhenAll(publisherTasks);
        }
        
        logger.LogInformation("ConnectorDataSyncRecurringJob Comp finished at {CurrentTimeUtc}",
            DateTimeOffset.UtcNow);
    }

    private async Task PublishSyncEvent(ConnectorDataSyncEvent syncEvent)
    {
        await channelWriter.WriteAsync(syncEvent);
    }
}