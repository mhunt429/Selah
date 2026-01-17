using System.Net;
using System.Text;
using System.Text.Json;
using AwesomeAssertions;
using Domain.ApiContracts.Identity;
using IntegrationTests.Helpers;

namespace IntegrationTests.Controller;

[Collection("Database")]
public class IdentityControllerTests(TestFactory factory, DatabaseFixture fixture)
    : IClassFixture<TestFactory>, IAsyncLifetime
{
    private string _email = $"{Guid.NewGuid().ToString()}@test.com";
    private string _password = "Testing0!";
    private string _jwtToken = string.Empty;
    private string _refreshToken = string.Empty;

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Login_ShouldReturnUnAuthorized_WhenInvalidCredentials()
    {
        var loginRequest = new LoginRequest
        {
            Email = "invalid@test.com",
            Password = "invalid",
            RememberMe = true
        };


        var response = await _client.PostAsync($"/api/identity/login", loginRequest.BuildRequestBody());
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenValidCredentials()
    {
        var loginRequest = new LoginRequest
        {
            Email = _email,
            Password = _password,
            RememberMe = true
        };

        var response = await _client.PostAsync($"/api/identity/login", loginRequest.BuildRequestBody());
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCurrentUser_Returns401WhenUnAuthenticated()
    {
        _client.ClearAuthHeader();
        var response = await _client.GetAsync("/api/identity/current-user");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCurrentUser_ReturnsSuccesshenUnAuthenticated()
    {
        _client.ClearAuthHeader();
        _client.GenerateClientHeaders(_jwtToken);
        var response = await _client.GetAsync("/api/identity/current-user");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task RefreshToken_Returns401WithInvalidToken()
    {
        var request = new RefreshTokenRequest
        {
            RefreshToken = "invalid"
        };

        var response = await _client.PostAsync("/api/identity/refresh-token",
            request.BuildRequestBody());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_ReturnsSuccessWithValidToken()
    {
        var request = new RefreshTokenRequest
        {
            RefreshToken = _refreshToken
        };

        var response = await _client.PostAsync("/api/identity/refresh-token",
            request.BuildRequestBody());

        response.EnsureSuccessStatusCode();
    }

    public async Task InitializeAsync()
    {
        await fixture.ResetDatabaseAsync();

        var tokens = await ApiTestHelpers.CreateTestUser(_client, _email, _password);

        _jwtToken = tokens.Item1;
        _refreshToken = tokens.Item2;

        _client.GenerateClientHeaders(_jwtToken);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}