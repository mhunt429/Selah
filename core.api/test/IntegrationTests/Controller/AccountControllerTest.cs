using System.Net;
using AwesomeAssertions;
using Domain.ApiContracts.AccountRegistration;
using IntegrationTests.Helpers;

namespace IntegrationTests.Controller;

[Collection("Database")]
public class AccountControllerTest(TestFactory factory, DatabaseFixture fixture) : IClassFixture<TestFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task UserShouldBeAbleToRegister()
    {
       string jwt = (await ApiTestHelpers.CreateTestUser(_client, $"{Guid.NewGuid().ToString()}@test.com", "Testing0!")).Item1;
       
       jwt.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task InvalidRegistrationShouldReturnBadRequest()
    {
        var request = new AccountRegistrationRequest
        {
            Email = "Email",
            Password = "password",
        };
        
        var response = await _client.PostAsync("/api/account/register", request.BuildRequestBody());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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