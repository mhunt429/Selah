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

    public async Task<ApiResponseResult<PlaidLinkToken>> GetLinkToken(int userId, bool updateMode = false)
    {
        var linkTokenRequest = new PlainLinkTokenRequest
        {
            ClientId = plaidConfig.ClientId,
            Secret = plaidConfig.ClientSecret,
            User = new PlaidUser { UserId = userId.ToString() }
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
        Uri tokenExchangeEndpoint = new Uri($"{httpClient.BaseAddress}/item/public_token/exchange");

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
        Uri balanceEndpoint = new Uri($"{httpClient.BaseAddress}/accounts/balance/get");

        var request = new PlaidAccountBalanceRequest()
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
}