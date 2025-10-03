using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;
using Akka;
using Domain.Models.Entities.AccountConnector;
using Infrastructure.Repository.Interfaces;
using Akka.Streams;
using Akka.Streams.Dsl;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.RecurringJobs;

public class ConnectorDataSyncRecurringJob : IJob
{
    private readonly ILogger<ConnectorDataSyncRecurringJob> _logger;
    private readonly IAccountConnectorRepository _connectorRepository;
    private readonly IPlaidAccountBalanceImportService _plaidAccountBalanceImportService;

    public ConnectorDataSyncRecurringJob(
        ILogger<ConnectorDataSyncRecurringJob> logger,
        IAccountConnectorRepository connectorRepository,
        IPlaidAccountBalanceImportService plaidAccountBalanceImportService)
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


        Source<ConnectionSyncDataEntity, NotUsed> source = Source.From(dbRecords);

        Source<IEnumerable<ConnectionSyncDataEntity>, NotUsed> batchingSource =
            source.Via(Flow.Create<ConnectionSyncDataEntity>().Grouped(100));

        await _plaidAccountBalanceImportService.ImportAccountBalancesAsync(batchingSource);


        _logger.LogInformation("ConnectorDataSyncRecurringJob Comp finished at {CurrentTimeUtc}",
            DateTimeOffset.UtcNow);
    }
}