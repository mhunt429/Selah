using System.Text.Json.Serialization;

namespace Domain.Models.Plaid;

public class PlaidLinkToken
{
    [JsonPropertyName("link_token")] public required string LinkToken { get; set; }
}

public class PlainLinkTokenRequest
{
    [JsonPropertyName("client_id")] public required string ClientId { get; set; }

    [JsonPropertyName("secret")] public required string Secret { get; set; }

    [JsonPropertyName("country_codes")] public List<string> CountryCodes { get; set; } = new List<string> { "US" };

    [JsonPropertyName("language")] public string Language { get; set; } = "en";

    [JsonPropertyName("products")]
    public List<string> Products { get; set; } = new List<string> { "auth", "transactions" };

    [JsonPropertyName("client_name")] public string ClientName { get; set; } = "Selah";

    [JsonPropertyName("user")] public required PlaidUser User { get; set; }
}

public class PlaidUser
{
    [JsonPropertyName("client_user_id")] public int UserId { get; set; }
}

public class PlaidTokenExchangeRequest
{
    [JsonPropertyName("client_id")] public required string ClientId { get; set; }

    [JsonPropertyName("secret")] public required string Secret { get; set; }

    [JsonPropertyName("public_token")] public required string PublicToken { get; set; }
}

public class PlaidTokenExchangeResponse
{
    [JsonPropertyName("access_token")] public required string AccessToken { get; set; }
    [JsonPropertyName("item_id")] public string ItemId { get; set; } = "";
}

//This is sent over on the success event handler from the frontend
public class TokenExchangeHttpRequest
{
    public required string PublicToken { get; set; }

    public int UserId { get; set; }

    public required string InstitutionName { get; set; }

    public required string InstitutionId { get; set; }
}