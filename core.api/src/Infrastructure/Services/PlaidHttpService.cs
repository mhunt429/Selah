using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Domain.Configuration;
using Domain.Models.Plaid;
using Infrastructure.Extensions;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.Services;

[ExcludeFromCodeCoverage]
public class PlaidHttpService : IPlaidHttpService
{
    private readonly HttpClient _httpClient;
    private readonly PlaidConfig _plaidConfig;
    private readonly ILogger<PlaidHttpService> _logger;

    public PlaidHttpService(HttpClient httpClient, PlaidConfig plaidConfig, ILogger<PlaidHttpService> logger)
    {
        _httpClient = httpClient;
        _plaidConfig = plaidConfig;
        _logger = logger;
    }

    public async Task<PlaidLinkToken?> GetLinkToken(Guid userId)
    {
        var linkTokenRequest = new PlainLinkTokenRequest
        {
            ClientId = _plaidConfig.ClientId,
            Secret = _plaidConfig.ClientSecret,
            User = new PlaidUser { UserId = userId }
        };

        Uri linkTokenEndpoint = new Uri($"{_httpClient.BaseAddress}link/token/create");

        HttpResponseMessage response = await _httpClient.PostAsync(linkTokenEndpoint, linkTokenRequest);
        var messageBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Link Token Create Request failed for user {userId} with status {statusCode} and error with {error}",
                userId, response.StatusCode, messageBody);

            return null;
        }

        return JsonSerializer.Deserialize<PlaidLinkToken>(messageBody);
    }

    public async Task<PlaidTokenExchangeResponse?> ExchangePublicToken(Guid userId, string publicToken)
    {
        Uri tokenExchangeEndpoint = new Uri($"{_httpClient.BaseAddress}/item/public_token/exchange");

        var tokenExchange = new PlaidTokenExchangeRequest
        {
            ClientId = _plaidConfig.ClientId,
            Secret = _plaidConfig.ClientSecret,
            PublicToken = publicToken
        };

        HttpResponseMessage response = await _httpClient.PostAsync(tokenExchangeEndpoint, tokenExchange);

        var messageBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Link Token Exchange Request failed for user {userId} with status {statusCode} and error with {error}",
                userId, response.StatusCode, messageBody);

            return null;
        }

        return JsonSerializer.Deserialize<PlaidTokenExchangeResponse>(messageBody);
    }
}