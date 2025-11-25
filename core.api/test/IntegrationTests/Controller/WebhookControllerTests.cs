using System.Net;
using AwesomeAssertions;
using Infrastructure.Extensions;
using IntegrationTests.Helpers;

namespace IntegrationTests.Controller;

public class WebhookControllerTests(TestFactory factory) : IClassFixture<TestFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Rate_Limit_Should_Block_After_ExceededLimit()
    {
        var defaultHttpContent = new StringContent("");
        for (int i = 0; i < 3; i++)
        {
            var response = await _client.PostAsync("/api/webhooks/plaid", defaultHttpContent);
            response.EnsureSuccessStatusCode();
        }

        var blocked = await _client.PostAsync("/api/webhooks/plaid", defaultHttpContent);
        blocked.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }
}