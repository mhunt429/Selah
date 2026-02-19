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
[ExcludeFromCodeCoverage(Justification = @"This class really just takes a response from Plaid and returns a ApiResponseResult<T>
based on the status of the response. Kind of trivial to test and also a pain to mock all the http message handler for 
EACH endpoint. Service that depend on this class have the right mocks setup to actually test the business logic")]
public class PlaidHttpService(HttpClient httpClient, PlaidConfig plaidConfig, ILogger<PlaidHttpService> logger)
    : IPlaidHttpService
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase // optional, Plaid API expects camelCase
    };

    //TODO add a new link for investment accounts

    public async Task<ApiResponseResult<PlaidLinkToken>> GetLinkToken(int userId, string? accessToken = null)
    {
        var linkTokenRequest = new PlainLinkTokenRequest
        {
            ClientId = plaidConfig.ClientId,
            Secret = plaidConfig.ClientSecret,
            User = new PlaidUser { UserId = userId.ToString() },
            Products = new List<string>() { "auth", "transactions" },
            Transactions = new LinkTokenTransactions
            {
                DaysRequested = plaidConfig.MaxDaysRequested
            },
            Webhook = !string.IsNullOrEmpty(plaidConfig.WebhookUrl) ? plaidConfig.WebhookUrl : null,

            AccessToken = accessToken
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
        string accessToken)
    {
        Uri recurringTransactionsEndpoint = new Uri($"{httpClient.BaseAddress}transactions/recurring/get");

        var request = new BasePlaidRequest
        {
            ClientId = plaidConfig.ClientId,
            Secret = plaidConfig.ClientSecret,
            AccessToken = accessToken
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

    public async Task<ApiResponseResult<GetItemResponse>> GetItem(BasePlaidRequest request)
    {
        Uri endpoint = new Uri($"{httpClient.BaseAddress}item/get");
        HttpResponseMessage response = await httpClient.PostAsync(endpoint, request);
        var messageBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError(
                "Get Item Request failed with status {statusCode} and error {error}",
                response.StatusCode, messageBody);
            return new ApiResponseResult<GetItemResponse>(ResultStatus.Failed, messageBody, null);
        }

        return new ApiResponseResult<GetItemResponse>(ResultStatus.Success, messageBody,
            JsonSerializer.Deserialize<GetItemResponse>(messageBody));
    }
}