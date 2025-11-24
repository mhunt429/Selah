using WebApi.IntegrationTests.Helpers;

namespace WebApi.IntegrationTests.Controller;

public class HealthCheckControllerUnitTests : IClassFixture<TestFactory>
{
    private readonly HttpClient _client;

    public HealthCheckControllerUnitTests(TestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HeathCheckEndpointShouldReturnOk()
    {
        var response = await _client.GetAsync("/hc");
        response.EnsureSuccessStatusCode();
    }
}