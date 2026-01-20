using System.Net;
using AwesomeAssertions;
using Domain.Models.Plaid;
using IntegrationTests.Helpers;

namespace IntegrationTests.Controller;

[Collection("Database")]
public class ConnectorControllerTests(TestFactory factory, DatabaseFixture fixture)
    : IClassFixture<TestFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private string _email = $"{Guid.NewGuid().ToString()}@test.com";
    private string _password = "Testing0!";

    private string _jwtToken = string.Empty;

    public async Task InitializeAsync()
    {
        await fixture.ResetDatabaseAsync();
        _jwtToken = (await ApiTestHelpers.CreateTestUser(_client, _email, _password)).Item1;
    }

    [Fact]
    public async Task GetLinkToken_ReturnsUnAuthorizedWhenUnAuthenticated()
    {
        _client.ClearAuthHeader();

        var response = await _client.GetAsync("/api/connector/link");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ExchangeToken_ReturnsUnAuthorizedWhenUnAuthenticated()
    {
        _client.ClearAuthHeader();

        var request = new TokenExchangeHttpRequest
        {
            PublicToken = "token-123",
            InstitutionName = "Wells Fargo",
            InstitutionId = "123"
        };

        var response = await _client.PostAsync("/api/connector/exchange", request.BuildRequestBody());
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}