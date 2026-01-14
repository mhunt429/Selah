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
    private readonly HttpClient _client = factory.CreateClient();


    private string _jwtToken;

    [Fact]
    public async Task Login_ShouldReturnUnAuthorized_WhenInvalidCredentials()
    {
        var loginRequest = new LoginRequest
        {
            Email = "invalid@test.com",
            Password = "invalid",
            RememberMe = true
        };

        var body = JsonSerializer.Serialize(loginRequest);
        var httpContent = new StringContent(body, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync($"/api/identity/login", httpContent);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    public async Task InitializeAsync()
    {
        await fixture.ResetDatabaseAsync();

        _jwtToken = await ApiTestHelpers.CreateTestUser(_client, $"{Guid.NewGuid().ToString()}@test.com", "Testing0!");
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}