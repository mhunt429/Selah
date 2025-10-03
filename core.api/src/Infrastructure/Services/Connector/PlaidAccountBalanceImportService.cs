using Akka;
using Akka.Streams;
using Akka.Streams.Dsl;
using Domain.Models.Entities.AccountConnector;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Connector;

public class PlaidAccountBalanceImportService : IPlaidAccountBalanceImportService
{
    private readonly IAccountConnectorRepository _accountConnectorRepository;
    private readonly IFinancialAccountRepository _financialAccountRepository;
    private readonly ILogger<PlaidAccountBalanceImportService> _logger;
    private readonly IPlaidHttpService _plaidHttpService;
    private readonly ICryptoService _cryptoService;
    private IMaterializer _materializer;

    public PlaidAccountBalanceImportService(IAccountConnectorRepository accountConnectorRepository,
        IPlaidHttpService plaidHttpService,
        IFinancialAccountRepository financialAccountRepository, ILogger<PlaidAccountBalanceImportService> logger,
        ICryptoService cryptoService, IMaterializer materializer)
    {
        _accountConnectorRepository = accountConnectorRepository;
        _plaidHttpService = plaidHttpService;
        _financialAccountRepository = financialAccountRepository;
        _logger = logger;
        _cryptoService = cryptoService;
        _materializer = materializer;
    }

    public async Task ImportAccountBalancesAsync(
        Source<IEnumerable<ConnectionSyncDataEntity>, NotUsed> recordsToSyncStream)
    {
        await recordsToSyncStream
            .SelectAsync(1, async batch =>
            {
                _logger.LogInformation(
                    "PlaidAccountBalanceImportService => Processing new batch of {BatchSize} records at {TimestampUtc}",
                    batch.Count(), DateTimeOffset.UtcNow);

                // Run the inner stream and return its Task<Done>
                var result = await Source.From(batch)
                    .SelectAsync(8, async record =>
                    {
                        // Fire-and-forget message to actor
                        //_connectorSyncProcessorActor.Tell(record);

                        await Task.CompletedTask;
                        return record; // return the record so the stream has something
                    })
                    .RunWith(Sink.Ignore<ConnectionSyncDataEntity>(), _materializer);

                _logger.LogInformation(
                    "PlaidAccountBalanceImportService => Finished batch of {BatchSize} records at {TimestampUtc}",
                    batch.Count(), DateTimeOffset.UtcNow);

                return result; // Task<Done>
            })
            .RunWith(Sink.Ignore<Done>(), _materializer); // consume outer stream
    }
}