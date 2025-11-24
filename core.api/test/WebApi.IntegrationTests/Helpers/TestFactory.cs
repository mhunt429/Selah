using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace WebApi.IntegrationTests.Helpers;

public class TestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var testConfig = new Dictionary<string, string?>
            {
                ["SelahDbConnectionString"] =
                    "User ID=postgres;Password=postgres;Host=localhost;Port=65432;Database=postgres",
                
                ["AwsConfig:AccessKey"] = "abc123",
                ["AwsConfig:SecretKey"] = "abc123",
                ["AwsConfig:Region"] = "us-east-1",
                
                ["PlaidConfig:ClientId"] = "abced",
                ["PlaidConfig:ClientSecret"] = "abced",
                ["PlaidConfig:BaseUrl"] = "https://sandbox.plaid.com",
                
                ["SecurityConfig:JwtSecret"] = "Don'tUseThisInProduction",
                ["SecurityConfig:HashIdSalt"] = "Don'tUseThisInProduction",
                ["SecurityConfig:CryptoSecret"] = "Don'tUseThisInProduction",
                ["SecurityConfig:AccessTokenExpiryMinutes"] = "30",
                ["SecurityConfig:RefreshTokenExpiryDays"] = "3",
                
                ["TwilioConfig:ApiToken"] = "123456789",
            
                ["QuartzConfig:AccountBalanceRefreshJobCronExpression"] = "0 */5 * * * ?",
            };
            

            config.AddInMemoryCollection(testConfig);
        });
        builder.ConfigureServices(services => { });
    }
}