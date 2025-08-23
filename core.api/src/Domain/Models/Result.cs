namespace Domain.Models;

public record ApiResponseResult<T>(ResultStatus status, string? message, T? data);

public enum ResultStatus
{
    Success,
    Failed
}