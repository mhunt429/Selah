using System.Net;
using AwesomeAssertions;
using IntegrationTests.Helpers;

namespace IntegrationTests.Controller;

[Collection("Database")]
public class BankingControllerTests(TestFactory factory, DatabaseFixture fixture)
    : IClassFixture<TestFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private string _email = $"{Guid.NewGuid().ToString()}@test.com";
    private string _password = "Testing0!";

    private string _jwtToken = string.Empty;

    [Fact]
    public async Task GetBankAccounts_ShouldReturnOkWhenAuthenticated()
    {
        _client.ClearAuthHeader();
        _client.GenerateClientHeaders(_jwtToken);
        var response = await _client.GetAsync("/api/banking/accounts");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetBankAccountShouldReturnUnAuthorizedWhenUnAuthenticated()
    {
        _client.ClearAuthHeader();
        var response = await _client.GetAsync("/api/banking/accounts");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    public async Task InitializeAsync()
    {
        await fixture.ResetDatabaseAsync();

        _jwtToken = (await ApiTestHelpers.CreateTestUser(_client, _email, _password)).Item1;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}