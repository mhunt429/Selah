using System.Text.Json.Serialization;

namespace Domain.Models.Plaid;

public class PlaidRecurringTransactionsResponse
{
    [JsonPropertyName("inflow_streams")] public required List<PlaidRecurringStream> InflowStreams { get; set; }

    [JsonPropertyName("outflow_streams")] public required List<PlaidRecurringStream> OutflowStreams { get; set; }

    [JsonPropertyName("personal_finance_category_version")]
    public string? PersonalFinanceCategoryVersion { get; set; }

    [JsonPropertyName("request_id")] public string RequestId { get; set; }

    [JsonPropertyName("updated_datetime")] public DateTime? UpdatedDatetime { get; set; }
}

public class PlaidRecurringStream
{
    [JsonPropertyName("account_id")] public required string AccountId { get; set; }

    [JsonPropertyName("average_amount")] public required PlaidRecurringAmount AverageAmount { get; set; }

    [JsonPropertyName("category")] public List<string>? Category { get; set; }

    [JsonPropertyName("category_id")] public string? CategoryId { get; set; }

    [JsonPropertyName("description")] public required string Description { get; set; }

    [JsonPropertyName("first_date")] public required string FirstDate { get; set; }

    [JsonPropertyName("frequency")] public string? Frequency { get; set; }

    [JsonPropertyName("is_active")] public bool IsActive { get; set; }

    [JsonPropertyName("is_user_modified")] public bool IsUserModified { get; set; }

    [JsonPropertyName("last_amount")] public required PlaidRecurringAmount LastAmount { get; set; }

    [JsonPropertyName("last_date")] public required string LastDate { get; set; }

    [JsonPropertyName("last_user_modified_datetime")]
    public DateTime? LastUserModifiedDatetime { get; set; }

    [JsonPropertyName("merchant_name")] public string? MerchantName { get; set; }

    [JsonPropertyName("personal_finance_category")]
    public PlaidPersonalFinanceCategory? PersonalFinanceCategory { get; set; }

    [JsonPropertyName("predicted_next_date")] public string? PredictedNextDate { get; set; }

    [JsonPropertyName("status")] public string? Status { get; set; }

    [JsonPropertyName("stream_id")] public required string StreamId { get; set; }

    [JsonPropertyName("transaction_ids")] public required List<string> TransactionIds { get; set; }
}

public class PlaidRecurringAmount
{
    [JsonPropertyName("amount")] public decimal Amount { get; set; }
}

