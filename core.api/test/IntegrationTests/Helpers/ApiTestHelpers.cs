using System.Text;
using System.Text.Json;
using Domain.ApiContracts;
using Domain.ApiContracts.AccountRegistration;
using Domain.ApiContracts.Identity;

namespace IntegrationTests.Helpers;

public static class ApiTestHelpers
{
    public static async Task<string> CreateTestUser(HttpClient client, string email, string password)
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
        
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();

        var test = responseString;
        
        BaseHttpResponse<AccessTokenResponse>  rsp = JsonSerializer.Deserialize<BaseHttpResponse<AccessTokenResponse>>(responseString);
        
        return rsp.Data.AccessToken;
    }
}