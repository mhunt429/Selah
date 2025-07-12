using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extensions;

public static class HttpClientExtensions
{
    public static Task<HttpResponseMessage> GetAsync(this HttpClient httpClient, Uri uri)
    {
        return httpClient.GetAsync(uri);
    }

    public static Task<HttpResponseMessage> PostAsync<T>(this HttpClient httpClient, Uri uri, T data)
        where T : class
    {
        var body = JsonSerializer.Serialize(data);

        var httpContent = new StringContent(body, Encoding.UTF8, "application/json");

        return httpClient.PostAsync(uri, httpContent);
    }

    public static Task<HttpResponseMessage> PutAsync<T>(this HttpClient httpClient, Uri uri, T data)
        where T : class
    {
        var body = JsonSerializer.Serialize(data);

        var httpContent = new StringContent(body, Encoding.UTF8, "application/json");

        return httpClient.PutAsync(uri, httpContent);
    }

    public static Task<HttpResponseMessage> DeleteAsync(this HttpClient httpClient, Uri uri)
    {
        return httpClient.DeleteAsync(uri);
    }

    public static void AddBearerToken(this HttpClient httpClient, string bearerToken)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
    }
}