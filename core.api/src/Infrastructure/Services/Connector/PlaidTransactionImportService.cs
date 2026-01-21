using Domain.Events;
using Domain.Models;
using Domain.Models.Entities.Transactions;
using Domain.Models.Plaid;
using Domain.Shared;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Connector;

public class PlaidTransactionImportService(
    ICryptoService cryptoService,
    IPlaidHttpService plaidHttpService,
    ILogger<PlaidTransactionImportService> logger,
    IAccountConnectorRepository accountConnectorRepository,
    ITransactionRepository transactionRepository): IPlaidTransactionImportService
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

        if (transactionsData.Added.Any())
        {
            await AddNewTransactionsAsync(transactionsData.Added, syncEvent.UserId);
        }

        if (transactionsData.Modified.Any())
        {
            await UpdateTransactionsAsync(transactionsData.Modified, syncEvent.UserId);
        }

        // Process removed transactions
        if (transactionsData.Removed.Any())
        {
            await transactionRepository.DeleteTransactionsInBulk(transactionsData.Modified
                    .Select(t => t.TransactionId)
                    .ToList(), syncEvent.UserId
            );
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

    private async Task AddNewTransactionsAsync(IReadOnlyCollection<PlaidTransaction> transactions, int userId)
    {
        var mappedTransactions = transactions.Select(t => MapPlaidTransaction(t, userId)).ToList();
        await transactionRepository.AddTransactionsInBulk(mappedTransactions);
        logger.LogInformation(
            "Adding new transactions for user {UserId} with {Count} transactions", userId, mappedTransactions.Count());
    }

    private async Task UpdateTransactionsAsync(IEnumerable<PlaidTransaction> transactions, int userId)
    {
        var mappedTransactions = transactions.Select(t =>
            MapPlaidTransaction(t, userId)).ToList();
        await transactionRepository.UpdateTransactionsInBulk(mappedTransactions, userId);
    }


    private TransactionEntity MapPlaidTransaction(PlaidTransaction plaidTransaction, int userId)
    {
        return new TransactionEntity
        {
            Amount = plaidTransaction.Amount,
            TransactionDate = DateUtilities.ParseStringAsDate(plaidTransaction.Date),
            MerchantName = plaidTransaction.MerchantName,
            MerchantLogoUrl = plaidTransaction.LogoUrl,
            ExternalTransactionId = plaidTransaction.TransactionId,
            UserId = userId,
            TransactionName = plaidTransaction.Name,
            Pending = plaidTransaction.Pending,
            ImportedDate = DateTimeOffset.UtcNow,
            LineItems = new List<TransactionLineItemEntity>()
            {
                new()
                {
                    Description = plaidTransaction.PersonalFinanceCategory != null
                        ? plaidTransaction.PersonalFinanceCategory.Primary
                        : "",
                    Amount = plaidTransaction.Amount,
                }
            }
        };
    }
}