using System.Net;
using System.Text;
using System.Text.Json;
using AwesomeAssertions;
using Domain.ApiContracts.Identity;
using IntegrationTests.Helpers;

namespace IntegrationTests.Controller;

public class IdentityControllerTests(TestFactory factory) : IClassFixture<TestFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Rate_Limit_Should_Block_After_Limit_Reached()
    {
        var loginRequest = new LoginRequest
        {
            Email = "invalid@test.com",
            Password = "invalid",
            RememberMe = true
        };

        var body = JsonSerializer.Serialize(loginRequest);
        var httpContent = new StringContent(body, Encoding.UTF8, "application/json");
        
        for (int i = 0; i < 3; i++)
        {
            var response = await _client.PostAsync($"/api/identity/login", httpContent);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        var blockResponse = await _client.PostAsync($"/api/identity/login", httpContent);
        blockResponse.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }
}