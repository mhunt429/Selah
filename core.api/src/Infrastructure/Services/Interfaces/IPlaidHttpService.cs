using Domain.Models;
using Domain.Models.Plaid;

namespace Infrastructure.Services.Interfaces;

public interface IPlaidHttpService
{
    Task<ApiResponseResult<PlaidLinkToken>> GetLinkToken(int userId, string? accessToken = null);

    Task<ApiResponseResult<PlaidTokenExchangeResponse>> ExchangePublicToken(int userId, string publicToken);

    Task<ApiResponseResult<PlaidBalanceApiResponse>> GeAccountBalance(string accessToken);

    Task<ApiResponseResult<PlaidTransactionsSyncResponse>> SyncTransactions(string accessToken, string? cursor = null, int count = 250);

    Task<ApiResponseResult<PlaidRecurringTransactionsResponse>> GetRecurringTransactions(string accessToken);

    Task<ApiResponseResult<PlaidWebhookVerificationResponse>> ValidateWebhook(string keyId);
}