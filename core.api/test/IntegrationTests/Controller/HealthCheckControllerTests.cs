using IntegrationTests.Helpers;

namespace IntegrationTests.Controller;

public class HealthCheckControllerTests(TestFactory factory) : IClassFixture<TestFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task HeathCheckEndpointShouldReturnOk()
    {
        var response = await _client.GetAsync("/hc");
        response.EnsureSuccessStatusCode();
    }
}