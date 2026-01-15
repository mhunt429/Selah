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

    [Fact]
    public async Task Login_ShouldReturnUnAuthorized_WhenInvalidCredentials()
    {
        // Create a unique client per test with a unique IP to avoid sharing rate limiter state
        var uniqueIp = $"192.168.2.{new Random().Next(100, 255)}";
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Forwarded-For", uniqueIp);
        
        var loginRequest = new LoginRequest
        {
            Email = "invalid@test.com",
            Password = "invalid",
            RememberMe = true
        };

        var body = JsonSerializer.Serialize(loginRequest);
        var httpContent = new StringContent(body, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"/api/identity/login", httpContent);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenValidCredentials()
    {
        // Create a unique client per test with a unique IP to avoid sharing rate limiter state
        var uniqueIp = $"192.168.3.{new Random().Next(100, 255)}";
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Forwarded-For", uniqueIp);
        
        var loginRequest = new LoginRequest
        {
            Email = _email,
            Password = _password,
            RememberMe = true
        };
        var body = JsonSerializer.Serialize(loginRequest);
        var httpContent = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"/api/identity/login", httpContent);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    public async Task InitializeAsync()
    {
        await fixture.ResetDatabaseAsync();

        // Create a unique client for InitializeAsync with a unique IP to avoid sharing rate limiter state
        var uniqueIp = $"192.168.4.{new Random().Next(100, 255)}";
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Forwarded-For", uniqueIp);
        
        _jwtToken = await ApiTestHelpers.CreateTestUser(client, _email, _password);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}