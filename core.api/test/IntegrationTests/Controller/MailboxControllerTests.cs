using System.Net;
using System.Text;
using System.Text.Json;
using AwesomeAssertions;
using Domain.ApiContracts.Mailbox;
using Infrastructure;
using Infrastructure.Repository;
using IntegrationTests.Helpers;

namespace IntegrationTests.Controller;

[Collection("Database")]
public class MailboxControllerTests(TestFactory factory, DatabaseFixture fixture)
    : IClassFixture<TestFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private string _email = $"{Guid.NewGuid().ToString()}@test.com";
    private string _password = "Testing0!";

    private string _jwtToken = string.Empty;


    [Fact]
    public async Task ControllerShouldReturn401WhenUnAuthenticated()
    {
        var response = await _client.GetAsync("/api/mailbox/messages");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMessages_ShouldReturnOkWhenAuthenticated()
    {
        _client.ClearAuthHeader();
        _client.GenerateClientHeaders(_jwtToken);
        var response = await _client.GetAsync("/api/mailbox/messages");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetMessageById_ShouldReturnOkWhenAuthenticated()
    {
        _client.ClearAuthHeader();
        _client.GenerateClientHeaders(_jwtToken);
        var response = await _client.GetAsync($"/api/mailbox/messages/{1}");

        response.EnsureSuccessStatusCode();
    }


    [Fact]
    public async Task DeleteMessage_ShouldReturnOkWhenAuthenticated()
    {
        _client.ClearAuthHeader();
        _client.GenerateClientHeaders(_jwtToken);
        var response = await _client.DeleteAsync($"/api/mailbox/messages/{1}");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task DeleteMessages_ShouldReturnOkWhenAuthenticated()
    {
        _client.ClearAuthHeader();
        _client.GenerateClientHeaders(_jwtToken);
        var response = await _client.DeleteAsync($"/api/mailbox/messages");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task MarkAsRead_ShouldReturnOkWhenAuthenticated()
    {
        _client.ClearAuthHeader();
        _client.GenerateClientHeaders(_jwtToken);
        var request = new MailboxUpdateRequest
        {
            MarkAsRead = true
        };

        var response = await _client.PutAsync($"/api/mailbox/messages/{1}", request.BuildRequestBody());

        response.EnsureSuccessStatusCode();
    }

    public async Task InitializeAsync()
    {
        await fixture.ResetDatabaseAsync();

        _jwtToken = (await ApiTestHelpers.CreateTestUser(_client, _email, _password)).Item1;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}