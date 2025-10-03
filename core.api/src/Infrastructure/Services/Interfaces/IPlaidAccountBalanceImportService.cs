using Akka;
using Akka.Streams.Dsl;
using Domain.Models.Entities.AccountConnector;

namespace Infrastructure.Services.Interfaces;

public interface IPlaidAccountBalanceImportService
{
    Task ImportAccountBalancesAsync(
        Source<IEnumerable<ConnectionSyncDataEntity>, NotUsed> recordsToSyncStream);
}