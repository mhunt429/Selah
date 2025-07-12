namespace Domain.Models;

public record ApiResponseResult<T>(ResultStatus status, string? message, T? data, IEnumerable<string>? errors = null);

public record DbOperationResult(ResultStatus status, string? message);

public enum ResultStatus
{
    Success,
    Failed
}