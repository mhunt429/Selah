using Domain.Events;
using Domain.Models;
using Domain.Models.Plaid;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Connector;

public class PlaidTransactionImportService(
    ICryptoService cryptoService,
    IPlaidHttpService plaidHttpService,
    ILogger<PlaidTransactionImportService> logger,
    IAccountConnectorRepository accountConnectorRepository)
{
    public async Task ImportTransactionsAsync(ConnectorDataSyncEvent syncEvent)
    {
        var accessToken = cryptoService.Decrypt(syncEvent.AccessToken);
        
        // Start with no cursor for initial sync
        await ImportTransactionsRecursiveAsync(syncEvent, accessToken, cursor: null);
        
        await accountConnectorRepository.UpdateConnectionSync(
            syncEvent.DataSyncId,
            syncEvent.UserId,
            DateTimeOffset.UtcNow.AddDays(3));
    }

    private async Task ImportTransactionsRecursiveAsync(
        ConnectorDataSyncEvent syncEvent,
        string accessToken,
        string? cursor)
    {
        ApiResponseResult<PlaidTransactionsSyncResponse> transactionsResponse =
            await plaidHttpService.SyncTransactions(accessToken, cursor);

        if (transactionsResponse.status == ResultStatus.Failed)
        {
            logger.LogError(
                "Transactions Sync failed for user {UserId} with error {ErrorMsg}",
                syncEvent.UserId,
                transactionsResponse.message);
            return;
        }

        PlaidTransactionsSyncResponse? transactionsData = transactionsResponse.data;

        if (transactionsData == null)
        {
            logger.LogWarning(
                "Transactions Sync returned null data for user {UserId}",
                syncEvent.UserId);
            return;
        }

        // Process added transactions
        if (transactionsData.Added.Any())
        {
            await ProcessTransactionsAsync(syncEvent, transactionsData.Added, "added");
        }

        // Process modified transactions
        if (transactionsData.Modified.Any())
        {
            await ProcessTransactionsAsync(syncEvent, transactionsData.Modified, "modified");
        }

        // Process removed transactions
        if (transactionsData.Removed.Any())
        {
            await ProcessTransactionsAsync(syncEvent, transactionsData.Removed, "removed");
        }

        // Continue pagination if there are more transactions
        if (transactionsData.HasMore && !string.IsNullOrEmpty(transactionsData.NextCursor))
        {
            logger.LogInformation(
                "More transactions available for user {UserId}. Continuing with cursor {Cursor}",
                syncEvent.UserId,
                transactionsData.NextCursor);

            await ImportTransactionsRecursiveAsync(syncEvent, accessToken, transactionsData.NextCursor);
        }
        else
        {
            logger.LogInformation(
                "Transaction import completed for user {UserId}. Total transactions processed.",
                syncEvent.UserId);
        }
    }

    private async Task ProcessTransactionsAsync(
        ConnectorDataSyncEvent syncEvent,
        List<PlaidTransaction> transactions,
        string action)
    {
        logger.LogInformation(
            "Processing {Count} {Action} transactions for user {UserId}",
            transactions.Count,
            action,
            syncEvent.UserId);

        // TODO: Add repository logic here to save transactions to database
        // For now, just log the transactions
        foreach (var transaction in transactions)
        {
            logger.LogDebug(
                "Transaction {TransactionId}: {Name} - {Amount} {Currency} on {Date}",
                transaction.TransactionId,
                transaction.Name,
                transaction.Amount,
                transaction.IsoCurrencyCode ?? "USD",
                transaction.Date);
        }

        await Task.CompletedTask;
    }
}

