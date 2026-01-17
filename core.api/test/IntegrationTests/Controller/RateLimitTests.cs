using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AwesomeAssertions;
using Domain.ApiContracts.Connector;
using IntegrationTests.Helpers;

namespace IntegrationTests.Controller;

[Collection("Database")]
public class RateLimitTests(TestFactory factory, DatabaseFixture fixture) : IClassFixture<TestFactory>, IAsyncLifetime
{
    private readonly TestFactory _factory = factory;
    private string _jwtToken = string.Empty;
    
    [Fact]
    public async Task PublicEndpointPolicy_Rate_Limit_Should_Block_After_ExceededLimit()
    {
        // Create a unique client per test with a unique IP to avoid sharing rate limiter state
        var uniqueIp = $"192.168.1.{new Random().Next(100, 255)}";
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Forwarded-For", uniqueIp);
        
        var request = new PlaidWebhookRequest
        {
            WebhookCode = "SYNC_UPDATES_AVAILABLE",
            ItemId = Guid.NewGuid().ToString(),
        };
        
        string jsonBody = JsonSerializer.Serialize(request);
        
        var defaultHttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        
        // For IntegrationTests, the limit is 3, so make 3 requests then expect the 4th to be blocked
        for (int i = 0; i < 20; i++)
        {
            var response = await client.PostAsync("/api/webhooks/plaid", defaultHttpContent);
            response.EnsureSuccessStatusCode();
        }

        var blocked = await client.PostAsync("/api/webhooks/plaid", defaultHttpContent);
        blocked.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }

    [Fact]
    public async Task PrivateEndpointPolicy_Rate_Limit_Should_Block_After_ExceededLimit()
    {
        // Create a unique client per test with a unique JWT token to avoid sharing rate limiter state
        var jwt = (await ApiTestHelpers.CreateTestUser(_factory.CreateClient(), $"{Guid.NewGuid().ToString()}@test.com", "Testing0!")).Item1;
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        
        for (int i = 0; i < 100; i++)
        {
            var response = await client.GetAsync("/api/identity/current-user");
            response.EnsureSuccessStatusCode();
        }
        
        var blocked = await client.GetAsync("/api/identity/current-user");
        blocked.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }


    public async Task InitializeAsync()
    {
        await fixture.ResetDatabaseAsync();

    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}