using Domain.Events;

namespace Infrastructure.Services.Interfaces;

public interface IPlaidTransactionImportService
{
    Task ImportTransactionsAsync(ConnectorDataSyncEvent syncEvent);

    Task ImportRecurringTransactionsAsync(ConnectorDataSyncEvent syncEvent);
}