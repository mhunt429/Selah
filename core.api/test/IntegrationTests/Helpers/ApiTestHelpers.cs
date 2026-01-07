using System.Text;
using System.Text.Json;
using Domain.ApiContracts.AccountRegistration;

namespace IntegrationTests.Helpers;

public static class ApiTestHelpers
{
    public static async Task CreateTestUser(HttpClient client, string email, string password)
    {
        AccountRegistrationRequest loginRequest = new()
        {
            Email = email,
            Password = password,
            FirstName = Guid.NewGuid().ToString(),
            LastName = Guid.NewGuid().ToString(),
            PasswordConfirmation = password,
            PhoneNumber = "1231231234",
        };
        var body = JsonSerializer.Serialize(loginRequest);
        var httpContent = new StringContent(body, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("api/account/register", httpContent);
        var responseString = await response.Content.ReadAsStringAsync();
        var test = responseString;
        response.EnsureSuccessStatusCode();
    }
}