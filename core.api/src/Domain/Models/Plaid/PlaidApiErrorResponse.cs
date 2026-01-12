using System.Text.Json.Serialization;

namespace Domain.Models.Plaid;

public class PlaidApiErrorResponse
{
    [JsonPropertyName("error_type")] public string? ErrorType { get; set; }

    [JsonPropertyName("error_code")] public string? ErrorCode { get; set; }

    [JsonPropertyName("error_message")] public string? ErrorMessage { get; set; }
}

public static class ErrorCodes
{
    public static readonly string LoginRequired = "ITEM_LOGIN_REQUIRED";
}