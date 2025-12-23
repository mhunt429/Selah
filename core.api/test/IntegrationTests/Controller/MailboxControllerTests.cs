using System.Net;
using AwesomeAssertions;
using Infrastructure;
using Infrastructure.Repository;
using IntegrationTests.Helpers;

namespace IntegrationTests.Controller;

[Collection("Database")]
public class MailboxControllerTests(TestFactory factory, DatabaseFixture fixture): IClassFixture<TestFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly AppDbContext _dbContext = TestHelpers.BuildTestDbContext();
    
    [Fact]
    public async Task ControllerShouldReturn401WhenUnAuthenticated()
    {
        var response = await _client.GetAsync("/api/mailbox/messages");
        
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    public async Task InitializeAsync()
    {
        await fixture.ResetDatabaseAsync();
        var result = await TestHelpers.SetUpBaseRecords();
    }

    public Task DisposeAsync()
    {
       return Task.CompletedTask;
    }
}