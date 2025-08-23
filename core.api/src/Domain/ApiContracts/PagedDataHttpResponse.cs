namespace Domain.ApiContracts;

public class PagedDataHttpResponse<T>
{
    public int Status { get; set; }

    public required IReadOnlyCollection<T> Data { get; set; }

    public string? NextPageLink { get; set; }

    public string? PreviousPageLink { get; set; }

    public int? NextCursor { get; set; }
}