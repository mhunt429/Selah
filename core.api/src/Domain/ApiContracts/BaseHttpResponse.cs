using System.Text.Json.Serialization;

namespace Domain.ApiContracts;

public class BaseHttpResponse<T>
{
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("errors")]
    public IEnumerable<string>? Errors { get; set; } = Enumerable.Empty<string>();
}