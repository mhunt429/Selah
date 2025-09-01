namespace Domain.ApiContracts;

public class BaseHttpResponse<T>
{
    public int StatusCode { get; set; }

    public T? Data { get; set; }

    public IEnumerable<string>? Errors { get; set; } = Enumerable.Empty<string>();
}