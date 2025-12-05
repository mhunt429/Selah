using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;
using Domain.Configuration;
using Domain.Models;
using Domain.Models.Plaid;
using Infrastructure.Services;

namespace Infrastructure.UnitTests.Services;

public class PlaidHttpServiceTests
{
    private readonly Mock<ILogger<PlaidHttpService>> _mockLogger;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly PlaidConfig _plaidConfig;
    private readonly PlaidHttpService _plaidHttpService;

    public PlaidHttpServiceTests()
    {
        _mockLogger = new Mock<ILogger<PlaidHttpService>>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://api.plaid.com/")
        };

        _plaidConfig = new PlaidConfig
        {
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            BaseUrl = _httpClient.BaseAddress.ToString()
        };

        _plaidHttpService = new PlaidHttpService(_httpClient, _plaidConfig, _mockLogger.Object);
    }

    [Fact]
    public async Task GetLinkToken_WhenSuccessful_ReturnsSuccessResult()
    {
        // Arrange
        var userId = 123;
        var expectedResponse = new PlaidLinkToken
        {
            LinkToken = "test-link-token",
        };

        var responseContent = JsonSerializer.Serialize(expectedResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString().Contains("link/token/create")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _plaidHttpService.GetLinkToken(userId);

        // Assert
        Assert.Equal(ResultStatus.Success, result.status);
        Assert.NotNull(result.data);
        Assert.Equal(expectedResponse.LinkToken, result.data.LinkToken);
    }

    [Fact]
    public async Task GetLinkToken_WhenHttpRequestFails_ReturnsFailedResult()
    {
        // Arrange
        var userId = 123;
        var errorMessage = "Bad Request";
        var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(errorMessage, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _plaidHttpService.GetLinkToken(userId);

        // Assert
        Assert.Equal(ResultStatus.Failed, result.status);
        Assert.Equal(errorMessage, result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task ExchangePublicToken_WhenSuccessful_ReturnsSuccessResult()
    {
        // Arrange
        var userId = 123;
        var publicToken = "public-test-token";
        var expectedResponse = new PlaidTokenExchangeResponse
        {
            AccessToken = "access-test-token",
            ItemId = "test-item-id"
        };

        var responseContent = JsonSerializer.Serialize(expectedResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString().Contains("item/public_token/exchange")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _plaidHttpService.ExchangePublicToken(userId, publicToken);

        // Assert
        Assert.Equal(ResultStatus.Success, result.status);
        Assert.NotNull(result.data);
        Assert.Equal(expectedResponse.AccessToken, result.data.AccessToken);
        Assert.Equal(expectedResponse.ItemId, result.data.ItemId);
    }

    [Fact]
    public async Task ExchangePublicToken_WhenHttpRequestFails_ReturnsFailedResult()
    {
        // Arrange
        var userId = 123;
        var publicToken = "public-test-token";
        var errorMessage = "Invalid public token";
        var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(errorMessage, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _plaidHttpService.ExchangePublicToken(userId, publicToken);

        // Assert
        Assert.Equal(ResultStatus.Failed, result.status);
        Assert.Equal(errorMessage, result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task GeAccountBalance_WhenSuccessful_ReturnsSuccessResult()
    {
        // Arrange
        var accessToken = "access-test-token";
        var expectedResponse = new PlaidBalanceApiResponse
        {
            Accounts = new List<PlaidAccountBalance>
            {
                new PlaidAccountBalance
                {
                    AccountId = "test-account-id",
                    Balance = new Balances
                    {
                        Available = 420.69m,
                        Current = 500,
                        IsoCurrencyCode = "USD"
                    }
                }
            }
        };

        var responseContent = JsonSerializer.Serialize(expectedResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString().Contains("accounts/balance/get")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        ApiResponseResult<PlaidBalanceApiResponse> result = await _plaidHttpService.GeAccountBalance(accessToken);

        // Assert
        Assert.Equal(ResultStatus.Success,
            result.status); // Note: The service returns Failed even on success - this might be a bug
        Assert.NotNull(result.data);
        Assert.Single(result.data.Accounts);
        Assert.Equal(expectedResponse.Accounts.FirstOrDefault().AccountId,
            result.data.Accounts.FirstOrDefault().AccountId);
        ;
    }

    [Fact]
    public async Task GeAccountBalance_WhenHttpRequestFails_ReturnsFailedResult()
    {
        // Arrange
        var accessToken = "access-test-token";
        var errorMessage = "Invalid access token";
        var httpResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent(errorMessage, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _plaidHttpService.GeAccountBalance(accessToken);

        // Assert
        Assert.Equal(ResultStatus.Failed, result.status);
        Assert.Equal(errorMessage, result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task GetLinkToken_VerifiesCorrectRequestPayload()
    {
        // Arrange
        var userId = 123;
        HttpRequestMessage capturedRequest = null;

        var response = new PlaidLinkToken
        {
            LinkToken = "test-link-token",
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(response), Encoding.UTF8, "application/json")
            });

        // Act
        await _plaidHttpService.GetLinkToken(userId);

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest.Method);
        Assert.Contains("link/token/create", capturedRequest.RequestUri.ToString());

        var requestContent = await capturedRequest.Content.ReadAsStringAsync();
        var requestPayload = JsonSerializer.Deserialize<PlainLinkTokenRequest>(requestContent);

        Assert.Equal(_plaidConfig.ClientId, requestPayload.ClientId);
        Assert.Equal(_plaidConfig.ClientSecret, requestPayload.Secret);
        Assert.Equal(userId.ToString(), requestPayload.User.UserId);
    }

    [Fact]
    public async Task ExchangePublicToken_VerifiesCorrectRequestPayload()
    {
        // Arrange
        var userId = 123;
        var publicToken = "test-public-token";
        HttpRequestMessage capturedRequest = null;

        PlaidTokenExchangeResponse response = new PlaidTokenExchangeResponse
        {
            AccessToken = "test-access-token",
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(response), Encoding.UTF8, "application/json")
            });

        // Act
        await _plaidHttpService.ExchangePublicToken(userId, publicToken);

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest.Method);
        Assert.Contains("item/public_token/exchange", capturedRequest.RequestUri.ToString());

        var requestContent = await capturedRequest.Content.ReadAsStringAsync();
        var requestPayload = JsonSerializer.Deserialize<PlaidTokenExchangeRequest>(requestContent);

        Assert.Equal(_plaidConfig.ClientId, requestPayload.ClientId);
        Assert.Equal(_plaidConfig.ClientSecret, requestPayload.Secret);
        Assert.Equal(publicToken, requestPayload.PublicToken);
    }

    [Fact]
    public async Task GeAccountBalance_VerifiesCorrectRequestPayload()
    {
        // Arrange
        var accessToken = "test-access-token";
        HttpRequestMessage capturedRequest = null;

        var response = new PlaidBalanceApiResponse
        {
            Accounts = new List<PlaidAccountBalance>
            {
                new PlaidAccountBalance()
                {
                    AccountId = "123",

                    Balance = new Balances
                    {
                        Available = 420.69m,
                        Current = 500,
                        IsoCurrencyCode = "USD"
                    }
                }
            }
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(response), Encoding.UTF8, "application/json")
            });

        // Act
        await _plaidHttpService.GeAccountBalance(accessToken);

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest.Method);
        Assert.Contains("accounts/balance/get", capturedRequest.RequestUri.ToString());

        var requestContent = await capturedRequest.Content.ReadAsStringAsync();
        var requestPayload = JsonSerializer.Deserialize<BasePlaidRequest>(requestContent);

        Assert.Equal(_plaidConfig.ClientId, requestPayload.ClientId);
        Assert.Equal(_plaidConfig.ClientSecret, requestPayload.Secret);
        Assert.Equal(accessToken, requestPayload.AccessToken);
    }
}