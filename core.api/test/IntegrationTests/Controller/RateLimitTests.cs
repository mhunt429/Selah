using System.Net;
using System.Text;
using System.Text.Json;
using AwesomeAssertions;
using Domain.ApiContracts.Connector;
using IntegrationTests.Helpers;

namespace IntegrationTests.Controller;

public class RateLimitTests(TestFactory factory) : IClassFixture<TestFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    
    [Fact]
    public async Task Rate_Limit_Should_Block_After_ExceededLimit()
    {
        var request = new PlaidWebhookRequest
        {
            WebhookCode = "SYNC_UPDATES_AVAILABLE",
            ItemId = Guid.NewGuid().ToString(),
        };
        
        string jsonBody = JsonSerializer.Serialize(request);
        
        var defaultHttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        for (int i = 0; i < 3; i++)
        {
            var response = await _client.PostAsync("/api/webhooks/plaid", defaultHttpContent);
            response.EnsureSuccessStatusCode();
        }

        var blocked = await _client.PostAsync("/api/webhooks/plaid", defaultHttpContent);
        blocked.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }
}