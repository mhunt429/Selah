using System.Text;
using System.Text.Json;
using Domain.ApiContracts;
using Domain.ApiContracts.AccountRegistration;
using Domain.ApiContracts.Identity;

namespace IntegrationTests.Helpers;

public static class ApiTestHelpers
{
    /// <summary>
    /// Returns a tuple of two string
    /// 1. JWT Access Token
    /// 2. Refresh Token
    /// </summary>
    /// <param name="client"></param>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static async Task<(string, string)> CreateTestUser(HttpClient client, string email, string password)
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
        
        return (rsp.Data.AccessToken, rsp.Data.RefreshToken);
    }

    public static void GenerateClientHeaders(this HttpClient client, string jwt = "")
    {
        var uniqueIp = $"192.168.3.{new Random().Next(100, 255)}";
        client.DefaultRequestHeaders.Add("X-Forwarded-For", uniqueIp);
        
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");
    }
    
    
    //In the event of parallel test execution or out-of-order test, clear this out so the headers don't bleed over
    public static void ClearAuthHeader(this HttpClient client)
    {
        client.DefaultRequestHeaders.Remove("Authorization");
    }
    
    public static StringContent BuildRequestBody<T>(this T body)
    {
        var json = JsonSerializer.Serialize(body);
       
       return new StringContent(json, Encoding.UTF8, "application/json");
    }
}