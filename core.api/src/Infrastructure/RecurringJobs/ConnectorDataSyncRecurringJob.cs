using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;
using Domain.Models.Entities.AccountConnector;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.RecurringJobs;

public class ConnectorDataSyncRecurringJob : IJob
{
    private readonly ILogger<ConnectorDataSyncRecurringJob> _logger;
    private readonly IAccountConnectorRepository _connectorRepository;

    public ConnectorDataSyncRecurringJob(
        ILogger<ConnectorDataSyncRecurringJob> logger,
        IAccountConnectorRepository connectorRepository)
    {
        _logger = logger;
        _connectorRepository  = connectorRepository;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("ConnectorDataSyncRecurringJob started at {CurrentTimeUtc}", DateTimeOffset.UtcNow);

        IEnumerable<ConnectionSyncDataEntity> dbRecords = await _connectorRepository.GetConnectorRecordsToImport();
        
        _logger.LogInformation("ConnectorDataSyncRecurringJob found {numRecords} connection records to import", dbRecords.Count());
        
        _logger.LogInformation("ConnectorDataSyncRecurringJob Comp finished at {CurrentTimeUtc}", DateTimeOffset.UtcNow);
    }
}