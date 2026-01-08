using System.Net;
using System.Text;
using System.Text.Json;
using AwesomeAssertions;
using Domain.ApiContracts.Connector;
using IntegrationTests.Helpers;

namespace IntegrationTests.Controller;

public class WebhookControllerTests(TestFactory factory) : IClassFixture<TestFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task PlaidWebhookEndpoint_ShouldReturn400_ForInvalidWebhookType()
    {
        var request = new PlaidWebhookRequest
        {
            WebhookCode = "Bad_Hook",
            ItemId = Guid.NewGuid().ToString(),
        };
        string jsonBody = JsonSerializer.Serialize(request);
        
        var defaultHttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/webhooks/plaid", defaultHttpContent);
        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().Be("Invalid Webhook Code");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}