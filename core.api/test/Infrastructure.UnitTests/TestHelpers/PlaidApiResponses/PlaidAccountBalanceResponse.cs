namespace Infrastructure.UnitTests.TestHelpers.PlaidApiResponses;

public static  class PlaidAccountBalanceResponse
{
    public static string HappyPathAccountBalance = @" 
{
    ""accounts"": [
        {
            ""account_id"": ""MeZj5wjbERcowQoQ7JKLCzkw3LgMl3uLXzX8j"",
            ""balances"": {
                ""available"": 100,
                ""current"": 110,
                ""iso_currency_code"": ""USD"",
                ""limit"": null,
                ""unofficial_currency_code"": null
            },
            ""mask"": ""0000"",
            ""name"": ""Plaid Checking"",
            ""official_name"": ""Plaid Gold Standard 0% Interest Checking"",
            ""persistent_account_id"": ""8cfb8beb89b774ee43b090625f0d61d0814322b43bff984eaf60386e"",
            ""subtype"": ""checking"",
            ""type"": ""depository""
        },
        {
            ""account_id"": ""1jw41y4KPLtmzEmErgVDCxMX48ezJ4cp939GA"",
            ""balances"": {
                ""available"": 200,
                ""current"": 210,
                ""iso_currency_code"": ""USD"",
                ""limit"": null,
                ""unofficial_currency_code"": null
            },
            ""mask"": ""1111"",
            ""name"": ""Plaid Saving"",
            ""official_name"": ""Plaid Silver Standard 0.1% Interest Saving"",
            ""persistent_account_id"": ""211a4e5d8361a3afb7a3886362198c7306e00a313b5aa944c20d34b6"",
            ""subtype"": ""savings"",
            ""type"": ""depository""
        }
    ],
    ""item"": {
        ""auth_method"": ""INSTANT_AUTH"",
        ""available_products"": [
            ""assets"",
            ""balance"",
            ""signal"",
            ""identity"",
            ""identity_match"",
            ""income_verification"",
            ""liabilities"",
            ""transactions_refresh""
        ],
        ""billed_products"": [
            ""auth"",
            ""transactions""
        ],
        ""consent_expiration_time"": null,
        ""consented_products"": [
            ""auth"",
            ""identity"",
            ""identity_match"",
            ""income_verification"",
            ""transactions""
        ],
        ""error"": null,
        ""institution_id"": ""ins_127989"",
        ""institution_name"": ""Bank of America"",
        ""item_id"": ""Bz5D6lD1rvCJDnJnWZRahPeEZAjxVQfwZDmGK"",
        ""products"": [
            ""auth"",
            ""transactions""
        ],
        ""update_type"": ""background"",
        ""webhook"": """"
    },
    ""request_id"": ""c6VrV8cFGCEqKU2""
}
";
}