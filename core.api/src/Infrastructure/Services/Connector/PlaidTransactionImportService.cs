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
        IReadOnlyCollection<FinancialAccountEntity?> financialAccounts =
            await financialAccountRepository.GetAccountsAsync(syncEvent.UserId, syncEvent.ConnectorId);

        bool hasMoreTransactions = true;
        string? cursor = syncEvent.TransactionSyncCursor;

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

            if (transactionsData.Added.Any())
            {
                await AddNewTransactionsAsync(transactionsData.Added, syncEvent.UserId, financialAccounts);
            }

            if (transactionsData.Modified.Any())
            {
                await UpdateTransactionsAsync(transactionsData.Modified, syncEvent.UserId, financialAccounts);
            }

            if (transactionsData.Removed.Any())
            {
                await transactionRepository.DeleteTransactionsInBulk(transactionsData.Modified
                        .Select(t => t.TransactionId)
                        .ToList(), syncEvent.UserId
                );
            }

            if (!string.IsNullOrEmpty(transactionsData.NextCursor))
            {
                cursor = transactionsData.NextCursor;
            }

            hasMoreTransactions = transactionsData.HasMore;
        }


        await accountConnectorRepository.UpdateConnectionSync(
            syncEvent.ConnectorId,
            syncEvent.UserId,
            DateTimeOffset.UtcNow.AddDays(3),
            cursor);
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
        for (int i = 0; i < financialAccounts.Count(); i++)
        {
            var externalId = financialAccounts.ElementAt(i).ExternalId;
            var accountId = financialAccounts.ElementAt(i).Id;

            var transactionToMap = transactions.FirstOrDefault(t => t.AccountId == externalId);

            if (transactionToMap != null)
            {
                mappedTransactions.Add(MapPlaidTransaction(transactionToMap, userId, accountId));
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