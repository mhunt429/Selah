using Domain.Events;
using Domain.Models;
using Domain.Models.Entities.FinancialAccount;
using Domain.Models.Entities.Transactions;
using Domain.Models.Plaid;
using Domain.Shared;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Connector;

public class PlaidTransactionImportService(
    ICryptoService cryptoService,
    IPlaidHttpService plaidHttpService,
    ILogger<PlaidTransactionImportService> logger,
    IAccountConnectorRepository accountConnectorRepository,
    ITransactionRepository transactionRepository,
    IFinancialAccountRepository financialAccountRepository) : IPlaidTransactionImportService
{
    public async Task ImportTransactionsAsync(ConnectorDataSyncEvent syncEvent)
    {
        var accessToken = cryptoService.Decrypt(syncEvent.AccessToken);

        //Go ahead and load existing accounts per connector so we can map a transaction to an account
        IReadOnlyCollection<FinancialAccountEntity?> nullableFinancialAccounts =
            await financialAccountRepository.GetAccountsAsync(syncEvent.UserId, syncEvent.ConnectorId);

        // Filter out null accounts and convert to non-nullable collection
        IReadOnlyCollection<FinancialAccountEntity> financialAccounts = nullableFinancialAccounts
            .Where(acc => acc != null)
            .Cast<FinancialAccountEntity>()
            .ToList();

        bool hasMoreTransactions = true;
        string? cursor = syncEvent.TransactionSyncCursor;

        int interation = 1;

        while (hasMoreTransactions)
        {
            ApiResponseResult<PlaidTransactionsSyncResponse> transactionsResponse =
                await plaidHttpService.SyncTransactions(accessToken, cursor);

            logger.LogInformation("Importing transactions for user {UserId} with cursor {Cursor}", syncEvent.UserId,
                cursor);

            if (transactionsResponse.status == ResultStatus.Failed)
            {
                logger.LogError(
                    "Transactions Sync failed for user {UserId} with error {ErrorMsg}",
                    syncEvent.UserId,
                    transactionsResponse.message);
                break;
            }

            if (transactionsResponse.data == null)
            {
                break;
            }

            var transactionsData = transactionsResponse.data;

            var addedTransactions = transactionsData.Added;
            var modifiedTransactions = transactionsData.Modified;
            var removedTransactions = transactionsData.Removed;

            logger.LogInformation(
                "Retrieved {NewTransactions} new transactions, {UpdatedTransactions} updated transactions, and {DeletedTransactions} deleted transactions for iteration {Iteration}",
                addedTransactions.Count, modifiedTransactions.Count, removedTransactions.Count, interation);

            if (addedTransactions.Any())
            {
                await AddNewTransactionsAsync(addedTransactions, syncEvent.UserId, financialAccounts);
            }

            if (modifiedTransactions.Any())
            {
                await UpdateTransactionsAsync(modifiedTransactions, syncEvent.UserId, financialAccounts);
            }

            if (removedTransactions.Any())
            {
                await transactionRepository.DeleteTransactionsInBulk(removedTransactions
                        .Select(t => t.TransactionId)
                        .ToList(), syncEvent.UserId
                );
            }

            if (!string.IsNullOrEmpty(transactionsData.NextCursor))
            {
                cursor = transactionsData.NextCursor;
            }

            hasMoreTransactions = transactionsData.HasMore;

            interation += 1;
        }


        logger.LogInformation("Finished syncing transactions for user {UserId}", syncEvent.UserId);

        await accountConnectorRepository.UpdateConnectionSyncCursor(
            syncEvent.ConnectorId,
            syncEvent.UserId,
            cursor
        );
    }

    private async Task AddNewTransactionsAsync(IReadOnlyCollection<PlaidTransaction> transactions, int userId,
        IReadOnlyCollection<FinancialAccountEntity> financialAccounts)
    {
        var mappedTransactions = MapTransactionsInBulk(transactions, userId, financialAccounts);
        if (mappedTransactions.Any())
        {
            await transactionRepository.AddTransactionsInBulk(mappedTransactions);
        }
    }

    private async Task UpdateTransactionsAsync(IEnumerable<PlaidTransaction> transactions, int userId,
        IReadOnlyCollection<FinancialAccountEntity> financialAccounts)
    {
        var mappedTransactions = MapTransactionsInBulk(transactions, userId, financialAccounts);
        if (mappedTransactions.Any())
        {
            await transactionRepository.UpdateTransactionsInBulk(mappedTransactions, userId);
        }
    }

    private IReadOnlyCollection<TransactionEntity> MapTransactionsInBulk(
        IEnumerable<PlaidTransaction> transactions, int userId,
        IReadOnlyCollection<FinancialAccountEntity> financialAccounts)
    {
        var mappedTransactions = new List<TransactionEntity>();
        
        var accountLookup = financialAccounts.ToDictionary(acc => acc.ExternalId, acc => acc.Id);
        
        foreach (var plaidTransaction in transactions)
        {
            if (accountLookup.TryGetValue(plaidTransaction.AccountId, out var accountId))
            {
                mappedTransactions.Add(MapPlaidTransaction(plaidTransaction, userId, accountId));
            }
            else
            {
                logger.LogWarning(
                    "No linked account found for transaction {TransactionId} with account {AccountId} for user {UserId}",
                    plaidTransaction.TransactionId, plaidTransaction.AccountId, userId);
            }
        }

        return mappedTransactions;
    }


    private TransactionEntity MapPlaidTransaction(PlaidTransaction plaidTransaction, int userId, int accountId)
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
            },
            AccountId = accountId,
        };
    }
}