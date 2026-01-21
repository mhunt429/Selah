using Domain.Events;

namespace Infrastructure.Services.Interfaces;

public interface IPlaidAccountBalanceImportService
{
    Task ImportAccountBalancesAsync(ConnectorDataSyncEvent syncEvent);
}