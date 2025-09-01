namespace Infrastructure.UnitTests.TestHelpers.PlaidApiResponses;

public static class PlaidTransactionResponse
{
    public static string HappyPathTransactionResponse = @"
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
    ""added"": [
        {
            ""account_id"": ""MeZj5wjbERcowQoQ7JKLCzkw3LgMl3uLXzX8j"",
            ""account_owner"": null,
            ""amount"": 6.33,
            ""authorized_date"": ""2025-01-01"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Travel"",
                ""Taxi""
            ],
            ""category_id"": ""22016000"",
            ""check_number"": null,
            ""counterparties"": [
                {
                    ""confidence_level"": ""VERY_HIGH"",
                    ""entity_id"": ""eyg8o776k0QmNgVpAmaQj4WgzW9Qzo6O51gdd"",
                    ""logo_url"": ""https://plaid-merchant-logos.plaid.com/uber_1060.png"",
                    ""name"": ""Uber"",
                    ""phone_number"": null,
                    ""type"": ""merchant"",
                    ""website"": ""uber.com""
                }
            ],
            ""date"": ""2025-01-02"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": ""https://plaid-merchant-logos.plaid.com/uber_1060.png"",
            ""merchant_entity_id"": ""eyg8o776k0QmNgVpAmaQj4WgzW9Qzo6O51gdd"",
            ""merchant_name"": ""Uber"",
            ""name"": ""Uber 072515 SF**POOL**"",
            ""payment_channel"": ""online"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""VERY_HIGH"",
                ""detailed"": ""TRANSPORTATION_TAXIS_AND_RIDE_SHARES"",
                ""primary"": ""TRANSPORTATION""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_TRANSPORTATION.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""Q1WX5VXv97HZXRZR7QkWCVz1ezZ3bniwj5eJ9"",
            ""transaction_type"": ""special"",
            ""unofficial_currency_code"": null,
            ""website"": ""uber.com""
        },
        {
            ""account_id"": ""MeZj5wjbERcowQoQ7JKLCzkw3LgMl3uLXzX8j"",
            ""account_owner"": null,
            ""amount"": 5.4,
            ""authorized_date"": ""2024-12-19"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Travel"",
                ""Taxi""
            ],
            ""category_id"": ""22016000"",
            ""check_number"": null,
            ""counterparties"": [
                {
                    ""confidence_level"": ""VERY_HIGH"",
                    ""entity_id"": ""eyg8o776k0QmNgVpAmaQj4WgzW9Qzo6O51gdd"",
                    ""logo_url"": ""https://plaid-merchant-logos.plaid.com/uber_1060.png"",
                    ""name"": ""Uber"",
                    ""phone_number"": null,
                    ""type"": ""merchant"",
                    ""website"": ""uber.com""
                }
            ],
            ""date"": ""2024-12-20"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": ""https://plaid-merchant-logos.plaid.com/uber_1060.png"",
            ""merchant_entity_id"": ""eyg8o776k0QmNgVpAmaQj4WgzW9Qzo6O51gdd"",
            ""merchant_name"": ""Uber"",
            ""name"": ""Uber 063015 SF**POOL**"",
            ""payment_channel"": ""online"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""VERY_HIGH"",
                ""detailed"": ""TRANSPORTATION_TAXIS_AND_RIDE_SHARES"",
                ""primary"": ""TRANSPORTATION""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_TRANSPORTATION.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""ZKj85g8rdpsWV6W6zygbtao3jo8dXACex6lN4"",
            ""transaction_type"": ""special"",
            ""unofficial_currency_code"": null,
            ""website"": ""uber.com""
        },
        {
            ""account_id"": ""MeZj5wjbERcowQoQ7JKLCzkw3LgMl3uLXzX8j"",
            ""account_owner"": null,
            ""amount"": -500,
            ""authorized_date"": ""2024-12-18"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Travel"",
                ""Airlines and Aviation Services""
            ],
            ""category_id"": ""22001000"",
            ""check_number"": null,
            ""counterparties"": [
                {
                    ""confidence_level"": ""VERY_HIGH"",
                    ""entity_id"": ""NKDjqyAdQQzpyeD8qpLnX0D6yvLe2KYKYYzQ4"",
                    ""logo_url"": ""https://plaid-merchant-logos.plaid.com/united_airlines_1065.png"",
                    ""name"": ""United Airlines"",
                    ""phone_number"": null,
                    ""type"": ""merchant"",
                    ""website"": ""united.com""
                }
            ],
            ""date"": ""2024-12-18"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": ""https://plaid-merchant-logos.plaid.com/united_airlines_1065.png"",
            ""merchant_entity_id"": ""NKDjqyAdQQzpyeD8qpLnX0D6yvLe2KYKYYzQ4"",
            ""merchant_name"": ""United Airlines"",
            ""name"": ""United Airlines"",
            ""payment_channel"": ""in store"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""VERY_HIGH"",
                ""detailed"": ""TRAVEL_FLIGHTS"",
                ""primary"": ""TRAVEL""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_TRAVEL.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""MeZj5wjbERcowQoQ7JKLCzkLWkgyDRfLmJyEE"",
            ""transaction_type"": ""special"",
            ""unofficial_currency_code"": null,
            ""website"": ""united.com""
        },
        {
            ""account_id"": ""MeZj5wjbERcowQoQ7JKLCzkw3LgMl3uLXzX8j"",
            ""account_owner"": null,
            ""amount"": 12,
            ""authorized_date"": ""2024-12-17"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Food and Drink"",
                ""Restaurants"",
                ""Fast Food""
            ],
            ""category_id"": ""13005032"",
            ""check_number"": null,
            ""counterparties"": [
                {
                    ""confidence_level"": ""VERY_HIGH"",
                    ""entity_id"": ""vzWXDWBjB06j5BJoD3Jo84OJZg7JJzmqOZA22"",
                    ""logo_url"": ""https://plaid-merchant-logos.plaid.com/mcdonalds_619.png"",
                    ""name"": ""McDonald's"",
                    ""phone_number"": null,
                    ""type"": ""merchant"",
                    ""website"": ""mcdonalds.com""
                }
            ],
            ""date"": ""2024-12-17"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": ""3322""
            },
            ""logo_url"": ""https://plaid-merchant-logos.plaid.com/mcdonalds_619.png"",
            ""merchant_entity_id"": ""vzWXDWBjB06j5BJoD3Jo84OJZg7JJzmqOZA22"",
            ""merchant_name"": ""McDonald's"",
            ""name"": ""McDonald's"",
            ""payment_channel"": ""in store"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""VERY_HIGH"",
                ""detailed"": ""FOOD_AND_DRINK_FAST_FOOD"",
                ""primary"": ""FOOD_AND_DRINK""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_FOOD_AND_DRINK.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""1jw41y4KPLtmzEmErgVDCxM8oMeA1GfpD8oxG"",
            ""transaction_type"": ""place"",
            ""unofficial_currency_code"": null,
            ""website"": ""mcdonalds.com""
        },
        {
            ""account_id"": ""MeZj5wjbERcowQoQ7JKLCzkw3LgMl3uLXzX8j"",
            ""account_owner"": null,
            ""amount"": 4.33,
            ""authorized_date"": ""2024-12-17"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Food and Drink"",
                ""Restaurants"",
                ""Coffee Shop""
            ],
            ""category_id"": ""13005043"",
            ""check_number"": null,
            ""counterparties"": [
                {
                    ""confidence_level"": ""VERY_HIGH"",
                    ""entity_id"": ""NZAJQ5wYdo1W1p39X5q5gpb15OMe39pj4pJBb"",
                    ""logo_url"": ""https://plaid-merchant-logos.plaid.com/starbucks_956.png"",
                    ""name"": ""Starbucks"",
                    ""phone_number"": null,
                    ""type"": ""merchant"",
                    ""website"": ""starbucks.com""
                }
            ],
            ""date"": ""2024-12-17"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": ""https://plaid-merchant-logos.plaid.com/starbucks_956.png"",
            ""merchant_entity_id"": ""NZAJQ5wYdo1W1p39X5q5gpb15OMe39pj4pJBb"",
            ""merchant_name"": ""Starbucks"",
            ""name"": ""Starbucks"",
            ""payment_channel"": ""in store"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""VERY_HIGH"",
                ""detailed"": ""FOOD_AND_DRINK_COFFEE"",
                ""primary"": ""FOOD_AND_DRINK""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_FOOD_AND_DRINK.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""L7b45j4ngQu4Gz4zygA1u14Xo4pa9MCk3XaRG"",
            ""transaction_type"": ""place"",
            ""unofficial_currency_code"": null,
            ""website"": ""starbucks.com""
        },
        {
            ""account_id"": ""MeZj5wjbERcowQoQ7JKLCzkw3LgMl3uLXzX8j"",
            ""account_owner"": null,
            ""amount"": 89.4,
            ""authorized_date"": ""2024-12-15"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Food and Drink"",
                ""Restaurants""
            ],
            ""category_id"": ""13005000"",
            ""check_number"": null,
            ""counterparties"": [
                {
                    ""confidence_level"": ""LOW"",
                    ""entity_id"": null,
                    ""logo_url"": null,
                    ""name"": ""FUN"",
                    ""phone_number"": null,
                    ""type"": ""merchant"",
                    ""website"": null
                }
            ],
            ""date"": ""2024-12-16"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": null,
            ""merchant_entity_id"": null,
            ""merchant_name"": ""FUN"",
            ""name"": ""SparkFun"",
            ""payment_channel"": ""in store"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""LOW"",
                ""detailed"": ""ENTERTAINMENT_SPORTING_EVENTS_AMUSEMENT_PARKS_AND_MUSEUMS"",
                ""primary"": ""ENTERTAINMENT""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_ENTERTAINMENT.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""p8XP3LPEalf68X6XGJvxC6AjwAdy9ztpXZnWV"",
            ""transaction_type"": ""place"",
            ""unofficial_currency_code"": null,
            ""website"": null
        },
        {
            ""account_id"": ""1jw41y4KPLtmzEmErgVDCxMX48ezJ4cp939GA"",
            ""account_owner"": null,
            ""amount"": 25,
            ""authorized_date"": ""2024-12-19"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Payment"",
                ""Credit Card""
            ],
            ""category_id"": ""16001000"",
            ""check_number"": null,
            ""counterparties"": [],
            ""date"": ""2024-12-20"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": null,
            ""merchant_entity_id"": null,
            ""merchant_name"": null,
            ""name"": ""CREDIT CARD 3333 PAYMENT *//"",
            ""payment_channel"": ""other"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""LOW"",
                ""detailed"": ""LOAN_PAYMENTS_CREDIT_CARD_PAYMENT"",
                ""primary"": ""LOAN_PAYMENTS""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_LOAN_PAYMENTS.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""ogLJBXJ8xWFD8gDgj5a3h5oKQopjJrsovdyzy"",
            ""transaction_type"": ""special"",
            ""unofficial_currency_code"": null,
            ""website"": null
        },
        {
            ""account_id"": ""1jw41y4KPLtmzEmErgVDCxMX48ezJ4cp939GA"",
            ""account_owner"": null,
            ""amount"": -4.22,
            ""authorized_date"": ""2024-12-15"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Transfer"",
                ""Payroll""
            ],
            ""category_id"": ""21009000"",
            ""check_number"": null,
            ""counterparties"": [],
            ""date"": ""2024-12-15"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": null,
            ""merchant_entity_id"": null,
            ""merchant_name"": null,
            ""name"": ""INTRST PYMNT"",
            ""payment_channel"": ""other"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""LOW"",
                ""detailed"": ""INCOME_WAGES"",
                ""primary"": ""INCOME""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_INCOME.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""geJP5APQvNcrV7r7Bqm9TPqwXqdGEbiEyx1w5"",
            ""transaction_type"": ""special"",
            ""unofficial_currency_code"": null,
            ""website"": null
        },
        {
            ""account_id"": ""MeZj5wjbERcowQoQ7JKLCzkw3LgMl3uLXzX8j"",
            ""account_owner"": null,
            ""amount"": 6.33,
            ""authorized_date"": ""2024-12-02"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Travel"",
                ""Taxi""
            ],
            ""category_id"": ""22016000"",
            ""check_number"": null,
            ""counterparties"": [
                {
                    ""confidence_level"": ""VERY_HIGH"",
                    ""entity_id"": ""eyg8o776k0QmNgVpAmaQj4WgzW9Qzo6O51gdd"",
                    ""logo_url"": ""https://plaid-merchant-logos.plaid.com/uber_1060.png"",
                    ""name"": ""Uber"",
                    ""phone_number"": null,
                    ""type"": ""merchant"",
                    ""website"": ""uber.com""
                }
            ],
            ""date"": ""2024-12-03"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": ""https://plaid-merchant-logos.plaid.com/uber_1060.png"",
            ""merchant_entity_id"": ""eyg8o776k0QmNgVpAmaQj4WgzW9Qzo6O51gdd"",
            ""merchant_name"": ""Uber"",
            ""name"": ""Uber 072515 SF**POOL**"",
            ""payment_channel"": ""online"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""VERY_HIGH"",
                ""detailed"": ""TRANSPORTATION_TAXIS_AND_RIDE_SHARES"",
                ""primary"": ""TRANSPORTATION""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_TRANSPORTATION.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""ZKj85g8rdpsWV6W6zygbtaoKWJrlVKsJpeoRz"",
            ""transaction_type"": ""special"",
            ""unofficial_currency_code"": null,
            ""website"": ""uber.com""
        },
        {
            ""account_id"": ""MeZj5wjbERcowQoQ7JKLCzkw3LgMl3uLXzX8j"",
            ""account_owner"": null,
            ""amount"": 5.4,
            ""authorized_date"": ""2024-11-19"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Travel"",
                ""Taxi""
            ],
            ""category_id"": ""22016000"",
            ""check_number"": null,
            ""counterparties"": [
                {
                    ""confidence_level"": ""VERY_HIGH"",
                    ""entity_id"": ""eyg8o776k0QmNgVpAmaQj4WgzW9Qzo6O51gdd"",
                    ""logo_url"": ""https://plaid-merchant-logos.plaid.com/uber_1060.png"",
                    ""name"": ""Uber"",
                    ""phone_number"": null,
                    ""type"": ""merchant"",
                    ""website"": ""uber.com""
                }
            ],
            ""date"": ""2024-11-20"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": ""https://plaid-merchant-logos.plaid.com/uber_1060.png"",
            ""merchant_entity_id"": ""eyg8o776k0QmNgVpAmaQj4WgzW9Qzo6O51gdd"",
            ""merchant_name"": ""Uber"",
            ""name"": ""Uber 063015 SF**POOL**"",
            ""payment_channel"": ""online"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""VERY_HIGH"",
                ""detailed"": ""TRANSPORTATION_TAXIS_AND_RIDE_SHARES"",
                ""primary"": ""TRANSPORTATION""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_TRANSPORTATION.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""MeZj5wjbERcowQoQ7JKLCzkqmrKb9qu4vLjlN"",
            ""transaction_type"": ""special"",
            ""unofficial_currency_code"": null,
            ""website"": ""uber.com""
        },
        {
            ""account_id"": ""MeZj5wjbERcowQoQ7JKLCzkw3LgMl3uLXzX8j"",
            ""account_owner"": null,
            ""amount"": -500,
            ""authorized_date"": ""2024-11-18"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Travel"",
                ""Airlines and Aviation Services""
            ],
            ""category_id"": ""22001000"",
            ""check_number"": null,
            ""counterparties"": [
                {
                    ""confidence_level"": ""VERY_HIGH"",
                    ""entity_id"": ""NKDjqyAdQQzpyeD8qpLnX0D6yvLe2KYKYYzQ4"",
                    ""logo_url"": ""https://plaid-merchant-logos.plaid.com/united_airlines_1065.png"",
                    ""name"": ""United Airlines"",
                    ""phone_number"": null,
                    ""type"": ""merchant"",
                    ""website"": ""united.com""
                }
            ],
            ""date"": ""2024-11-18"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": ""https://plaid-merchant-logos.plaid.com/united_airlines_1065.png"",
            ""merchant_entity_id"": ""NKDjqyAdQQzpyeD8qpLnX0D6yvLe2KYKYYzQ4"",
            ""merchant_name"": ""United Airlines"",
            ""name"": ""United Airlines"",
            ""payment_channel"": ""in store"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""VERY_HIGH"",
                ""detailed"": ""TRAVEL_FLIGHTS"",
                ""primary"": ""TRAVEL""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_TRAVEL.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""1jw41y4KPLtmzEmErgVDCxMpaLdl5puqRpnKD"",
            ""transaction_type"": ""special"",
            ""unofficial_currency_code"": null,
            ""website"": ""united.com""
        },
        {
            ""account_id"": ""MeZj5wjbERcowQoQ7JKLCzkw3LgMl3uLXzX8j"",
            ""account_owner"": null,
            ""amount"": 12,
            ""authorized_date"": ""2024-11-17"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Food and Drink"",
                ""Restaurants"",
                ""Fast Food""
            ],
            ""category_id"": ""13005032"",
            ""check_number"": null,
            ""counterparties"": [
                {
                    ""confidence_level"": ""VERY_HIGH"",
                    ""entity_id"": ""vzWXDWBjB06j5BJoD3Jo84OJZg7JJzmqOZA22"",
                    ""logo_url"": ""https://plaid-merchant-logos.plaid.com/mcdonalds_619.png"",
                    ""name"": ""McDonald's"",
                    ""phone_number"": null,
                    ""type"": ""merchant"",
                    ""website"": ""mcdonalds.com""
                }
            ],
            ""date"": ""2024-11-17"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": ""3322""
            },
            ""logo_url"": ""https://plaid-merchant-logos.plaid.com/mcdonalds_619.png"",
            ""merchant_entity_id"": ""vzWXDWBjB06j5BJoD3Jo84OJZg7JJzmqOZA22"",
            ""merchant_name"": ""McDonald's"",
            ""name"": ""McDonald's"",
            ""payment_channel"": ""in store"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""VERY_HIGH"",
                ""detailed"": ""FOOD_AND_DRINK_FAST_FOOD"",
                ""primary"": ""FOOD_AND_DRINK""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_FOOD_AND_DRINK.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""L7b45j4ngQu4Gz4zygA1u14NE5y3bNseMkAQB"",
            ""transaction_type"": ""place"",
            ""unofficial_currency_code"": null,
            ""website"": ""mcdonalds.com""
        },
        {
            ""account_id"": ""MeZj5wjbERcowQoQ7JKLCzkw3LgMl3uLXzX8j"",
            ""account_owner"": null,
            ""amount"": 4.33,
            ""authorized_date"": ""2024-11-17"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Food and Drink"",
                ""Restaurants"",
                ""Coffee Shop""
            ],
            ""category_id"": ""13005043"",
            ""check_number"": null,
            ""counterparties"": [
                {
                    ""confidence_level"": ""VERY_HIGH"",
                    ""entity_id"": ""NZAJQ5wYdo1W1p39X5q5gpb15OMe39pj4pJBb"",
                    ""logo_url"": ""https://plaid-merchant-logos.plaid.com/starbucks_956.png"",
                    ""name"": ""Starbucks"",
                    ""phone_number"": null,
                    ""type"": ""merchant"",
                    ""website"": ""starbucks.com""
                }
            ],
            ""date"": ""2024-11-17"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": ""https://plaid-merchant-logos.plaid.com/starbucks_956.png"",
            ""merchant_entity_id"": ""NZAJQ5wYdo1W1p39X5q5gpb15OMe39pj4pJBb"",
            ""merchant_name"": ""Starbucks"",
            ""name"": ""Starbucks"",
            ""payment_channel"": ""in store"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""VERY_HIGH"",
                ""detailed"": ""FOOD_AND_DRINK_COFFEE"",
                ""primary"": ""FOOD_AND_DRINK""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_FOOD_AND_DRINK.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""p8XP3LPEalf68X6XGJvxC6AKXWEvbKiJGp7r8"",
            ""transaction_type"": ""place"",
            ""unofficial_currency_code"": null,
            ""website"": ""starbucks.com""
        },
        {
            ""account_id"": ""MeZj5wjbERcowQoQ7JKLCzkw3LgMl3uLXzX8j"",
            ""account_owner"": null,
            ""amount"": 89.4,
            ""authorized_date"": ""2024-11-15"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Food and Drink"",
                ""Restaurants""
            ],
            ""category_id"": ""13005000"",
            ""check_number"": null,
            ""counterparties"": [
                {
                    ""confidence_level"": ""LOW"",
                    ""entity_id"": null,
                    ""logo_url"": null,
                    ""name"": ""FUN"",
                    ""phone_number"": null,
                    ""type"": ""merchant"",
                    ""website"": null
                }
            ],
            ""date"": ""2024-11-16"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": null,
            ""merchant_entity_id"": null,
            ""merchant_name"": ""FUN"",
            ""name"": ""SparkFun"",
            ""payment_channel"": ""in store"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""LOW"",
                ""detailed"": ""ENTERTAINMENT_SPORTING_EVENTS_AMUSEMENT_PARKS_AND_MUSEUMS"",
                ""primary"": ""ENTERTAINMENT""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_ENTERTAINMENT.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""ogLJBXJ8xWFD8gDgj5a3h5oB9EkvLBfpDoJMw"",
            ""transaction_type"": ""place"",
            ""unofficial_currency_code"": null,
            ""website"": null
        },
        {
            ""account_id"": ""MeZj5wjbERcowQoQ7JKLCzkw3LgMl3uLXzX8j"",
            ""account_owner"": null,
            ""amount"": 6.33,
            ""authorized_date"": ""2024-11-02"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Travel"",
                ""Taxi""
            ],
            ""category_id"": ""22016000"",
            ""check_number"": null,
            ""counterparties"": [
                {
                    ""confidence_level"": ""VERY_HIGH"",
                    ""entity_id"": ""eyg8o776k0QmNgVpAmaQj4WgzW9Qzo6O51gdd"",
                    ""logo_url"": ""https://plaid-merchant-logos.plaid.com/uber_1060.png"",
                    ""name"": ""Uber"",
                    ""phone_number"": null,
                    ""type"": ""merchant"",
                    ""website"": ""uber.com""
                }
            ],
            ""date"": ""2024-11-03"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": ""https://plaid-merchant-logos.plaid.com/uber_1060.png"",
            ""merchant_entity_id"": ""eyg8o776k0QmNgVpAmaQj4WgzW9Qzo6O51gdd"",
            ""merchant_name"": ""Uber"",
            ""name"": ""Uber 072515 SF**POOL**"",
            ""payment_channel"": ""online"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""VERY_HIGH"",
                ""detailed"": ""TRANSPORTATION_TAXIS_AND_RIDE_SHARES"",
                ""primary"": ""TRANSPORTATION""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_TRANSPORTATION.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""geJP5APQvNcrV7r7Bqm9TPqZep4jyZuLBEeVa"",
            ""transaction_type"": ""special"",
            ""unofficial_currency_code"": null,
            ""website"": ""uber.com""
        },
        {
            ""account_id"": ""MeZj5wjbERcowQoQ7JKLCzkw3LgMl3uLXzX8j"",
            ""account_owner"": null,
            ""amount"": 5.4,
            ""authorized_date"": ""2024-10-20"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Travel"",
                ""Taxi""
            ],
            ""category_id"": ""22016000"",
            ""check_number"": null,
            ""counterparties"": [
                {
                    ""confidence_level"": ""VERY_HIGH"",
                    ""entity_id"": ""eyg8o776k0QmNgVpAmaQj4WgzW9Qzo6O51gdd"",
                    ""logo_url"": ""https://plaid-merchant-logos.plaid.com/uber_1060.png"",
                    ""name"": ""Uber"",
                    ""phone_number"": null,
                    ""type"": ""merchant"",
                    ""website"": ""uber.com""
                }
            ],
            ""date"": ""2024-10-21"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": ""https://plaid-merchant-logos.plaid.com/uber_1060.png"",
            ""merchant_entity_id"": ""eyg8o776k0QmNgVpAmaQj4WgzW9Qzo6O51gdd"",
            ""merchant_name"": ""Uber"",
            ""name"": ""Uber 063015 SF**POOL**"",
            ""payment_channel"": ""online"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""VERY_HIGH"",
                ""detailed"": ""TRANSPORTATION_TAXIS_AND_RIDE_SHARES"",
                ""primary"": ""TRANSPORTATION""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_TRANSPORTATION.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""8LRDkwDaWvuoN1o1vVEXCAxKXBqZjKUZ6WP1P"",
            ""transaction_type"": ""special"",
            ""unofficial_currency_code"": null,
            ""website"": ""uber.com""
        },
        {
            ""account_id"": ""MeZj5wjbERcowQoQ7JKLCzkw3LgMl3uLXzX8j"",
            ""account_owner"": null,
            ""amount"": -500,
            ""authorized_date"": ""2024-10-19"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Travel"",
                ""Airlines and Aviation Services""
            ],
            ""category_id"": ""22001000"",
            ""check_number"": null,
            ""counterparties"": [
                {
                    ""confidence_level"": ""VERY_HIGH"",
                    ""entity_id"": ""NKDjqyAdQQzpyeD8qpLnX0D6yvLe2KYKYYzQ4"",
                    ""logo_url"": ""https://plaid-merchant-logos.plaid.com/united_airlines_1065.png"",
                    ""name"": ""United Airlines"",
                    ""phone_number"": null,
                    ""type"": ""merchant"",
                    ""website"": ""united.com""
                }
            ],
            ""date"": ""2024-10-19"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": ""https://plaid-merchant-logos.plaid.com/united_airlines_1065.png"",
            ""merchant_entity_id"": ""NKDjqyAdQQzpyeD8qpLnX0D6yvLe2KYKYYzQ4"",
            ""merchant_name"": ""United Airlines"",
            ""name"": ""United Airlines"",
            ""payment_channel"": ""in store"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""VERY_HIGH"",
                ""detailed"": ""TRAVEL_FLIGHTS"",
                ""primary"": ""TRAVEL""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_TRAVEL.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""Exrw5NwoanuQa1Q1bgvPirbgjNx7Zgtg74aRp"",
            ""transaction_type"": ""special"",
            ""unofficial_currency_code"": null,
            ""website"": ""united.com""
        },
        {
            ""account_id"": ""MeZj5wjbERcowQoQ7JKLCzkw3LgMl3uLXzX8j"",
            ""account_owner"": null,
            ""amount"": 12,
            ""authorized_date"": ""2024-10-18"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Food and Drink"",
                ""Restaurants"",
                ""Fast Food""
            ],
            ""category_id"": ""13005032"",
            ""check_number"": null,
            ""counterparties"": [
                {
                    ""confidence_level"": ""VERY_HIGH"",
                    ""entity_id"": ""vzWXDWBjB06j5BJoD3Jo84OJZg7JJzmqOZA22"",
                    ""logo_url"": ""https://plaid-merchant-logos.plaid.com/mcdonalds_619.png"",
                    ""name"": ""McDonald's"",
                    ""phone_number"": null,
                    ""type"": ""merchant"",
                    ""website"": ""mcdonalds.com""
                }
            ],
            ""date"": ""2024-10-18"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": ""3322""
            },
            ""logo_url"": ""https://plaid-merchant-logos.plaid.com/mcdonalds_619.png"",
            ""merchant_entity_id"": ""vzWXDWBjB06j5BJoD3Jo84OJZg7JJzmqOZA22"",
            ""merchant_name"": ""McDonald's"",
            ""name"": ""McDonald's"",
            ""payment_channel"": ""in store"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""VERY_HIGH"",
                ""detailed"": ""FOOD_AND_DRINK_FAST_FOOD"",
                ""primary"": ""FOOD_AND_DRINK""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_FOOD_AND_DRINK.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""WE4L58LlKpsqXVqVBz7LC34wXZMmWwHP86d9e"",
            ""transaction_type"": ""place"",
            ""unofficial_currency_code"": null,
            ""website"": ""mcdonalds.com""
        },
        {
            ""account_id"": ""MeZj5wjbERcowQoQ7JKLCzkw3LgMl3uLXzX8j"",
            ""account_owner"": null,
            ""amount"": 4.33,
            ""authorized_date"": ""2024-10-18"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Food and Drink"",
                ""Restaurants"",
                ""Coffee Shop""
            ],
            ""category_id"": ""13005043"",
            ""check_number"": null,
            ""counterparties"": [
                {
                    ""confidence_level"": ""VERY_HIGH"",
                    ""entity_id"": ""NZAJQ5wYdo1W1p39X5q5gpb15OMe39pj4pJBb"",
                    ""logo_url"": ""https://plaid-merchant-logos.plaid.com/starbucks_956.png"",
                    ""name"": ""Starbucks"",
                    ""phone_number"": null,
                    ""type"": ""merchant"",
                    ""website"": ""starbucks.com""
                }
            ],
            ""date"": ""2024-10-18"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": ""https://plaid-merchant-logos.plaid.com/starbucks_956.png"",
            ""merchant_entity_id"": ""NZAJQ5wYdo1W1p39X5q5gpb15OMe39pj4pJBb"",
            ""merchant_name"": ""Starbucks"",
            ""name"": ""Starbucks"",
            ""payment_channel"": ""in store"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""VERY_HIGH"",
                ""detailed"": ""FOOD_AND_DRINK_COFFEE"",
                ""primary"": ""FOOD_AND_DRINK""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_FOOD_AND_DRINK.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""Ar9xRQxMq4FXlkXkywbdc7RagmN9Aas4D9bjG"",
            ""transaction_type"": ""place"",
            ""unofficial_currency_code"": null,
            ""website"": ""starbucks.com""
        },
        {
            ""account_id"": ""MeZj5wjbERcowQoQ7JKLCzkw3LgMl3uLXzX8j"",
            ""account_owner"": null,
            ""amount"": 89.4,
            ""authorized_date"": ""2024-10-16"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Food and Drink"",
                ""Restaurants""
            ],
            ""category_id"": ""13005000"",
            ""check_number"": null,
            ""counterparties"": [
                {
                    ""confidence_level"": ""LOW"",
                    ""entity_id"": null,
                    ""logo_url"": null,
                    ""name"": ""FUN"",
                    ""phone_number"": null,
                    ""type"": ""merchant"",
                    ""website"": null
                }
            ],
            ""date"": ""2024-10-17"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": null,
            ""merchant_entity_id"": null,
            ""merchant_name"": ""FUN"",
            ""name"": ""SparkFun"",
            ""payment_channel"": ""in store"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""LOW"",
                ""detailed"": ""GENERAL_MERCHANDISE_OTHER_GENERAL_MERCHANDISE"",
                ""primary"": ""GENERAL_MERCHANDISE""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_GENERAL_MERCHANDISE.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""Gaky53y8jRsbd4b4X6w7CPzyXd5RKyuGv6V9b"",
            ""transaction_type"": ""place"",
            ""unofficial_currency_code"": null,
            ""website"": null
        },
        {
            ""account_id"": ""1jw41y4KPLtmzEmErgVDCxMX48ezJ4cp939GA"",
            ""account_owner"": null,
            ""amount"": 25,
            ""authorized_date"": ""2024-11-19"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Payment"",
                ""Credit Card""
            ],
            ""category_id"": ""16001000"",
            ""check_number"": null,
            ""counterparties"": [],
            ""date"": ""2024-11-20"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": null,
            ""merchant_entity_id"": null,
            ""merchant_name"": null,
            ""name"": ""CREDIT CARD 3333 PAYMENT *//"",
            ""payment_channel"": ""other"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""LOW"",
                ""detailed"": ""LOAN_PAYMENTS_CREDIT_CARD_PAYMENT"",
                ""primary"": ""LOAN_PAYMENTS""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_LOAN_PAYMENTS.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""mp9gkLgorafAvJAJNL7zCNqK8eMzaKi4NgGjW"",
            ""transaction_type"": ""special"",
            ""unofficial_currency_code"": null,
            ""website"": null
        },
        {
            ""account_id"": ""1jw41y4KPLtmzEmErgVDCxMX48ezJ4cp939GA"",
            ""account_owner"": null,
            ""amount"": -4.22,
            ""authorized_date"": ""2024-11-15"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Transfer"",
                ""Payroll""
            ],
            ""category_id"": ""21009000"",
            ""check_number"": null,
            ""counterparties"": [],
            ""date"": ""2024-11-15"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": null,
            ""merchant_entity_id"": null,
            ""merchant_name"": null,
            ""name"": ""INTRST PYMNT"",
            ""payment_channel"": ""other"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""LOW"",
                ""detailed"": ""INCOME_WAGES"",
                ""primary"": ""INCOME""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_INCOME.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""yxbMe3MvADu1we1eG65vfdnpo1bvkpFKm4Jdb"",
            ""transaction_type"": ""special"",
            ""unofficial_currency_code"": null,
            ""website"": null
        },
        {
            ""account_id"": ""1jw41y4KPLtmzEmErgVDCxMX48ezJ4cp939GA"",
            ""account_owner"": null,
            ""amount"": 25,
            ""authorized_date"": ""2024-10-20"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Payment"",
                ""Credit Card""
            ],
            ""category_id"": ""16001000"",
            ""check_number"": null,
            ""counterparties"": [],
            ""date"": ""2024-10-21"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": null,
            ""merchant_entity_id"": null,
            ""merchant_name"": null,
            ""name"": ""CREDIT CARD 3333 PAYMENT *//"",
            ""payment_channel"": ""other"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""LOW"",
                ""detailed"": ""LOAN_PAYMENTS_CREDIT_CARD_PAYMENT"",
                ""primary"": ""LOAN_PAYMENTS""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_LOAN_PAYMENTS.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""9pwdAVd4G5f9Pb9bk8DltL1Pq65R4Ptq1478a"",
            ""transaction_type"": ""special"",
            ""unofficial_currency_code"": null,
            ""website"": null
        },
        {
            ""account_id"": ""1jw41y4KPLtmzEmErgVDCxMX48ezJ4cp939GA"",
            ""account_owner"": null,
            ""amount"": -4.22,
            ""authorized_date"": ""2024-10-16"",
            ""authorized_datetime"": null,
            ""category"": [
                ""Transfer"",
                ""Payroll""
            ],
            ""category_id"": ""21009000"",
            ""check_number"": null,
            ""counterparties"": [],
            ""date"": ""2024-10-16"",
            ""datetime"": null,
            ""iso_currency_code"": ""USD"",
            ""location"": {
                ""address"": null,
                ""city"": null,
                ""country"": null,
                ""lat"": null,
                ""lon"": null,
                ""postal_code"": null,
                ""region"": null,
                ""store_number"": null
            },
            ""logo_url"": null,
            ""merchant_entity_id"": null,
            ""merchant_name"": null,
            ""name"": ""INTRST PYMNT"",
            ""payment_channel"": ""other"",
            ""payment_meta"": {
                ""by_order_of"": null,
                ""payee"": null,
                ""payer"": null,
                ""payment_method"": null,
                ""payment_processor"": null,
                ""ppd_id"": null,
                ""reason"": null,
                ""reference_number"": null
            },
            ""pending"": false,
            ""pending_transaction_id"": null,
            ""personal_finance_category"": {
                ""confidence_level"": ""LOW"",
                ""detailed"": ""INCOME_WAGES"",
                ""primary"": ""INCOME""
            },
            ""personal_finance_category_icon_url"": ""https://plaid-category-icons.plaid.com/PFC_INCOME.png"",
            ""transaction_code"": null,
            ""transaction_id"": ""vzRMP5Mmp3CnvMnMmlqjtxA4dmnjl4uvmqLxk"",
            ""transaction_type"": ""special"",
            ""unofficial_currency_code"": null,
            ""website"": null
        }
    ],
    ""has_more"": false,
    ""modified"": [],
    ""next_cursor"": ""CAESJXZ6Uk1QNU1tcDNDbnZNbk1tbHFqdHhBNGRtbmpsNHV2bXFMeGsaDAjXgYe8BhCI5eeyAiIMCNeBh7wGEIjl57ICKgwI14GHvAYQiOXnsgI="",
    ""removed"": [],
    ""request_id"": ""o9SEMuhwZVmwQ41"",
    ""transactions_update_status"": ""HISTORICAL_UPDATE_COMPLETE""
}
";
}