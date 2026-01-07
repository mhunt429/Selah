using IntegrationTests.Helpers;

namespace IntegrationTests.Controller;

public class AccountControllerTest(TestFactory factory) : IClassFixture<TestFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task UserShouldBeAbleToRegister()
    {
        await ApiTestHelpers.CreateTestUser(_client, $"{Guid.NewGuid().ToString()}@test.com", "Testing0!");
    }
}