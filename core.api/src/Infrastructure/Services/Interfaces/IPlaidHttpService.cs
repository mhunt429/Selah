using Domain.Models;
using Domain.Models.Plaid;

namespace Infrastructure.Services.Interfaces;

public interface IPlaidHttpService
{
    Task<ApiResponseResult<PlaidLinkToken>> GetLinkToken(int userId, bool updateMode = false);

    Task<ApiResponseResult<PlaidTokenExchangeResponse>> ExchangePublicToken(int userId, string publicToken);

    Task<ApiResponseResult<PlaidBalanceApiResponse>> GeAccountBalance(string accessToken);

    Task<ApiResponseResult<PlaidTransactionsSyncResponse>> SyncTransactions(string accessToken, string? cursor = null, int? count = null);

    Task<ApiResponseResult<PlaidRecurringTransactionsResponse>> GetRecurringTransactions(string accessToken);

    Task<ApiResponseResult<PlaidWebhookVerificationResponse>> ValidateWebhook(string keyId);
}