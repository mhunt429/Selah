namespace Domain.Models;

public record ApiResponseResult<T>(ResultStatus status, string? message, T? data, IEnumerable<string>? errors = null);

public enum ResultStatus
{
    Success,
    Failed
}