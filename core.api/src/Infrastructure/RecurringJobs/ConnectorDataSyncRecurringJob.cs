using Microsoft.Extensions.Logging;
using Quartz;
using Domain.Models.Entities.AccountConnector;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Connector;

namespace Infrastructure.RecurringJobs;

public class ConnectorDataSyncRecurringJob : IJob
{
    private readonly ILogger<ConnectorDataSyncRecurringJob> _logger;
    private readonly IAccountConnectorRepository _connectorRepository;
    private readonly PlaidAccountBalanceImportService _plaidAccountBalanceImportService;

    public ConnectorDataSyncRecurringJob(
        ILogger<ConnectorDataSyncRecurringJob> logger,
        IAccountConnectorRepository connectorRepository,
        PlaidAccountBalanceImportService plaidAccountBalanceImportService)
    {
        _logger = logger;
        _connectorRepository = connectorRepository;
        _plaidAccountBalanceImportService = plaidAccountBalanceImportService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("ConnectorDataSyncRecurringJob started at {CurrentTimeUtc}", DateTimeOffset.UtcNow);

        IEnumerable<ConnectionSyncDataEntity> dbRecords = await _connectorRepository.GetConnectorRecordsToImport();

        _logger.LogInformation("ConnectorDataSyncRecurringJob found {numRecords} connection records to import",
            dbRecords.Count());

        
       // await _plaidAccountBalanceImportService.ImportAccountBalancesAsync(batchingSource);


        _logger.LogInformation("ConnectorDataSyncRecurringJob Comp finished at {CurrentTimeUtc}",
            DateTimeOffset.UtcNow);
    }
}