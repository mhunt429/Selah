using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Domain.Configuration;
using Domain.Models;
using Domain.Models.Plaid;
using Infrastructure.Extensions;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.Services;

public class PlaidHttpService(HttpClient httpClient, PlaidConfig plaidConfig, ILogger<PlaidHttpService> logger)
    : IPlaidHttpService
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase // optional, Plaid API expects camelCase
    };

    public async Task<ApiResponseResult<PlaidLinkToken>> GetLinkToken(int userId, bool updateMode = false,
        bool initialLink = true)
    {
        var linkTokenRequest = new PlainLinkTokenRequest
        {
            ClientId = plaidConfig.ClientId,
            Secret = plaidConfig.ClientSecret,
            User = new PlaidUser { UserId = userId.ToString() },
            Products = initialLink
                ? new List<string>() { "auth", "transactions" }
                : new List<string>() { "investments" },
            Transactions = new LinkTokenTransactions
            {
                DaysRequested = plaidConfig.MaxDaysRequested
            },
            Webhook = !string.IsNullOrEmpty(plaidConfig.WebhookUrl) ? plaidConfig.WebhookUrl : null,
        };

        Uri linkTokenEndpoint = new Uri($"{httpClient.BaseAddress}link/token/create");

        HttpResponseMessage response = await httpClient.PostAsync(linkTokenEndpoint, linkTokenRequest);
        var messageBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError(
                "Link Token Create Request failed for user {userId} with status {statusCode} and error with {error}",
                userId, response.StatusCode, messageBody);

            return new ApiResponseResult<PlaidLinkToken>(ResultStatus.Failed, messageBody, null);
        }


        return new ApiResponseResult<PlaidLinkToken>(ResultStatus.Success, messageBody,
            JsonSerializer.Deserialize<PlaidLinkToken>(messageBody));
    }

    public async Task<ApiResponseResult<PlaidTokenExchangeResponse>> ExchangePublicToken(int userId, string publicToken)
    {
        Uri tokenExchangeEndpoint = new Uri($"{httpClient.BaseAddress}item/public_token/exchange");

        var tokenExchange = new PlaidTokenExchangeRequest
        {
            ClientId = plaidConfig.ClientId,
            Secret = plaidConfig.ClientSecret,
            PublicToken = publicToken
        };

        HttpResponseMessage response = await httpClient.PostAsync(tokenExchangeEndpoint, tokenExchange);

        var messageBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError(
                "Link Token Exchange Request failed for user {userId} with status {statusCode} and error with {error}",
                userId, response.StatusCode, messageBody);

            return new ApiResponseResult<PlaidTokenExchangeResponse>(ResultStatus.Failed, messageBody, null);
        }

        return new ApiResponseResult<PlaidTokenExchangeResponse>(ResultStatus.Success, messageBody,
            JsonSerializer.Deserialize<PlaidTokenExchangeResponse>(messageBody));
    }

    public async Task<ApiResponseResult<PlaidBalanceApiResponse>> GeAccountBalance(string accessToken)
    {
        Uri balanceEndpoint = new Uri($"{httpClient.BaseAddress}accounts/balance/get");

        var request = new BasePlaidRequest()
        {
            ClientId = plaidConfig.ClientId,
            Secret = plaidConfig.ClientSecret,
            AccessToken = accessToken
        };

        HttpResponseMessage response = await httpClient.PostAsync(balanceEndpoint, request);

        var messageBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponseResult<PlaidBalanceApiResponse>(ResultStatus.Failed, messageBody, null);
        }

        return new ApiResponseResult<PlaidBalanceApiResponse>(ResultStatus.Success, messageBody,
            JsonSerializer.Deserialize<PlaidBalanceApiResponse>(messageBody));
    }


    public async Task<ApiResponseResult<PlaidTransactionsSyncResponse>> SyncTransactions(string accessToken,
        string? cursor = null, int count = 50)
    {
        Uri transactionsSyncEndpoint = new Uri($"{httpClient.BaseAddress}transactions/sync");

        var request = new PlaidTransactionsSyncRequest
        {
            ClientId = plaidConfig.ClientId,
            Secret = plaidConfig.ClientSecret,
            AccessToken = accessToken,
            Cursor = cursor,
            Count = count
        };

        HttpResponseMessage response = await httpClient.PostAsync(transactionsSyncEndpoint, request);

        var messageBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError(
                "Transactions Sync Request failed with status {statusCode} and error {error}",
                response.StatusCode, messageBody);

            return new ApiResponseResult<PlaidTransactionsSyncResponse>(ResultStatus.Failed, messageBody, null);
        }

        return new ApiResponseResult<PlaidTransactionsSyncResponse>(ResultStatus.Success, messageBody,
            JsonSerializer.Deserialize<PlaidTransactionsSyncResponse>(messageBody));
    }

    public async Task<ApiResponseResult<PlaidRecurringTransactionsResponse>> GetRecurringTransactions(
        string accessToken, List<string> accountIds)
    {
        Uri recurringTransactionsEndpoint = new Uri($"{httpClient.BaseAddress}transactions/recurring/get");

        var request = new BasePlaidRequest
        {
            ClientId = plaidConfig.ClientId,
            Secret = plaidConfig.ClientSecret,
            AccessToken = accessToken,
            AccountIds = accountIds
        };

        HttpResponseMessage response = await httpClient.PostAsync(recurringTransactionsEndpoint, request);

        var messageBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError(
                "Recurring Transactions Request failed with status {statusCode} and error {error}",
                response.StatusCode, messageBody);

            return new ApiResponseResult<PlaidRecurringTransactionsResponse>(ResultStatus.Failed, messageBody, null);
        }

        return new ApiResponseResult<PlaidRecurringTransactionsResponse>(ResultStatus.Success, messageBody,
            JsonSerializer.Deserialize<PlaidRecurringTransactionsResponse>(messageBody));
    }

    public async Task<ApiResponseResult<PlaidWebhookVerificationResponse>> ValidateWebhook(string keyId)
    {
        Uri verificationEndpoint = new Uri($"{httpClient.BaseAddress}webhook_verification_key/get");

        var request = new WebhookVerificationRequest
        {
            KeyId = keyId,
            ClientId = plaidConfig.ClientId,
            Secret = plaidConfig.ClientSecret,
        };

        HttpResponseMessage response = await httpClient.PostAsync(verificationEndpoint, request);
        var messageBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError(
                "Webhook Verification Request failed with status {statusCode} and error {error}",
                response.StatusCode, messageBody);
            return new ApiResponseResult<PlaidWebhookVerificationResponse>(ResultStatus.Failed, messageBody, null);
        }

        return new ApiResponseResult<PlaidWebhookVerificationResponse>(ResultStatus.Success, messageBody,
            JsonSerializer.Deserialize<PlaidWebhookVerificationResponse>(messageBody));
    }
}