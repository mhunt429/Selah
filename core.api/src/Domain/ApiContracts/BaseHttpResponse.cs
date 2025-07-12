namespace Domain.ApiContracts;

public class BaseHttpResponse<T>
{
    public int StatusCode { get; set; }

    public T? Data { get; set; }

    public IEnumerable<string>? Errors{ get; set; } = Enumerable.Empty<string>();
}

/*
 * Most API commands can simply return the newly created or updated id so this
   provides a common interface of accomplishing that
 */
public class BaseCommandResponse
{
    public string Id { get; set; }
}