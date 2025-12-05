using System.Text.Json.Serialization;

namespace Domain.Models.Plaid;

public class PlaidTransactionsSyncResponse
{
    [JsonPropertyName("accounts")] public required List<PlaidTransactionAccount> Accounts { get; set; }

    [JsonPropertyName("added")] public required List<PlaidTransaction> Added { get; set; }

    [JsonPropertyName("has_more")] public bool HasMore { get; set; }

    [JsonPropertyName("modified")] public required List<PlaidTransaction> Modified { get; set; }

    [JsonPropertyName("next_cursor")] public string? NextCursor { get; set; }

    [JsonPropertyName("removed")] public required List<PlaidTransaction> Removed { get; set; }

    [JsonPropertyName("request_id")] public string RequestId { get; set; }

    [JsonPropertyName("transactions_update_status")]
    public string TransactionsUpdateStatus { get; set; }
}

public class PlaidTransactionAccount
{
    [JsonPropertyName("account_id")] public required string AccountId { get; set; }

    [JsonPropertyName("balances")] public required PlaidTransactionBalances Balances { get; set; }

    [JsonPropertyName("holder_category")] public string? HolderCategory { get; set; }

    [JsonPropertyName("mask")] public string? Mask { get; set; }

    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("official_name")] public string? OfficialName { get; set; }

    [JsonPropertyName("subtype")] public string? Subtype { get; set; }

    [JsonPropertyName("type")] public string? Type { get; set; }
}

public class PlaidTransactionBalances
{
    [JsonPropertyName("available")] public decimal? Available { get; set; }

    [JsonPropertyName("current")] public decimal? Current { get; set; }

    [JsonPropertyName("iso_currency_code")] public string? IsoCurrencyCode { get; set; }

    [JsonPropertyName("limit")] public decimal? Limit { get; set; }

    [JsonPropertyName("unofficial_currency_code")] public string? UnofficialCurrencyCode { get; set; }
}

public class PlaidTransaction
{
    [JsonPropertyName("account_id")] public required string AccountId { get; set; }

    [JsonPropertyName("account_owner")] public string? AccountOwner { get; set; }

    [JsonPropertyName("amount")] public decimal Amount { get; set; }

    [JsonPropertyName("authorized_date")] public string? AuthorizedDate { get; set; }

    [JsonPropertyName("authorized_datetime")] public DateTime? AuthorizedDatetime { get; set; }

    [JsonPropertyName("category")] public List<string>? Category { get; set; }

    [JsonPropertyName("category_id")] public string? CategoryId { get; set; }

    [JsonPropertyName("check_number")] public string? CheckNumber { get; set; }

    [JsonPropertyName("counterparties")] public required List<PlaidTransactionCounterparty> Counterparties { get; set; }

    [JsonPropertyName("date")] public required string Date { get; set; }

    [JsonPropertyName("datetime")] public DateTime? Datetime { get; set; }

    [JsonPropertyName("iso_currency_code")] public string? IsoCurrencyCode { get; set; }

    [JsonPropertyName("location")] public PlaidTransactionLocation? Location { get; set; }

    [JsonPropertyName("logo_url")] public string? LogoUrl { get; set; }

    [JsonPropertyName("merchant_entity_id")] public string? MerchantEntityId { get; set; }

    [JsonPropertyName("merchant_name")] public string? MerchantName { get; set; }

    [JsonPropertyName("name")] public required string Name { get; set; }

    [JsonPropertyName("payment_channel")] public string? PaymentChannel { get; set; }

    [JsonPropertyName("payment_meta")] public PlaidTransactionPaymentMeta? PaymentMeta { get; set; }

    [JsonPropertyName("pending")] public bool Pending { get; set; }

    [JsonPropertyName("pending_transaction_id")] public string? PendingTransactionId { get; set; }

    [JsonPropertyName("personal_finance_category")]
    public PlaidPersonalFinanceCategory? PersonalFinanceCategory { get; set; }

    [JsonPropertyName("personal_finance_category_icon_url")]
    public string? PersonalFinanceCategoryIconUrl { get; set; }

    [JsonPropertyName("transaction_code")] public string? TransactionCode { get; set; }

    [JsonPropertyName("transaction_id")] public required string TransactionId { get; set; }

    [JsonPropertyName("transaction_type")] public string? TransactionType { get; set; }

    [JsonPropertyName("unofficial_currency_code")]
    public string? UnofficialCurrencyCode { get; set; }

    [JsonPropertyName("website")] public string? Website { get; set; }
}

public class PlaidTransactionLocation
{
    [JsonPropertyName("address")] public string? Address { get; set; }

    [JsonPropertyName("city")] public string? City { get; set; }

    [JsonPropertyName("country")] public string? Country { get; set; }

    [JsonPropertyName("lat")] public decimal? Lat { get; set; }

    [JsonPropertyName("lon")] public decimal? Lon { get; set; }

    [JsonPropertyName("postal_code")] public string? PostalCode { get; set; }

    [JsonPropertyName("region")] public string? Region { get; set; }

    [JsonPropertyName("store_number")] public string? StoreNumber { get; set; }
}

public class PlaidTransactionPaymentMeta
{
    [JsonPropertyName("by_order_of")] public string? ByOrderOf { get; set; }

    [JsonPropertyName("payee")] public string? Payee { get; set; }

    [JsonPropertyName("payer")] public string? Payer { get; set; }

    [JsonPropertyName("payment_method")] public string? PaymentMethod { get; set; }

    [JsonPropertyName("payment_processor")] public string? PaymentProcessor { get; set; }

    [JsonPropertyName("ppd_id")] public string? PpdId { get; set; }

    [JsonPropertyName("reason")] public string? Reason { get; set; }

    [JsonPropertyName("reference_number")] public string? ReferenceNumber { get; set; }
}

public class PlaidTransactionCounterparty
{
    [JsonPropertyName("confidence_level")] public string? ConfidenceLevel { get; set; }

    [JsonPropertyName("entity_id")] public string? EntityId { get; set; }

    [JsonPropertyName("logo_url")] public string? LogoUrl { get; set; }

    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("phone_number")] public string? PhoneNumber { get; set; }

    [JsonPropertyName("type")] public string? Type { get; set; }

    [JsonPropertyName("website")] public string? Website { get; set; }
}

public class PlaidPersonalFinanceCategory
{
    [JsonPropertyName("confidence_level")] public string? ConfidenceLevel { get; set; }

    [JsonPropertyName("detailed")] public string? Detailed { get; set; }

    [JsonPropertyName("primary")] public string? Primary { get; set; }

    [JsonPropertyName("version")] public string? Version { get; set; }
}

