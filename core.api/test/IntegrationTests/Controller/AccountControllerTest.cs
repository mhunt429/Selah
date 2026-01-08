using IntegrationTests.Helpers;

namespace IntegrationTests.Controller;

[Collection("Database")]
public class AccountControllerTest(TestFactory factory, DatabaseFixture fixture) : IClassFixture<TestFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task UserShouldBeAbleToRegister()
    {
        await ApiTestHelpers.CreateTestUser(_client, $"{Guid.NewGuid().ToString()}@test.com", "Testing0!");
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