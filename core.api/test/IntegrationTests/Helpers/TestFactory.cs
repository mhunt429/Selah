using System.Net.Http.Headers;
using System.Text;
using System.Threading.RateLimiting;
using Domain.Configuration;
using Domain.Shared;
using Infrastructure;
using Infrastructure.Services;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using WebApi;
using WebApi.Middleware;

namespace IntegrationTests.Helpers;

public class TestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IDbConnectionFactory>(provider => new SelahDbConnectionFactory("User ID=postgres;Password=postgres;Host=localhost;Port=65432;Database=postgres"));

            services.AddSingleton(new AwsConfig
            {
                AccessKey = "abc123",
                SecretAccessKey = "abc123",
                Region = "us-east-1"
            });

            services.AddSingleton(new PlaidConfig
            {
                ClientId = "abced",
                ClientSecret = "abced",
                BaseUrl = "https://sandbox.plaid.com"
            });

            // Use a consistent JWT secret for both token generation and validation
            const string testJwtSecret = "DontUseThisInProductionDontUseThisInProductionDontUseThisInProductionDontUseThisInProduction";
            
            services.AddSingleton(new SecurityConfig
            {
                JwtSecret = testJwtSecret,
                HashIdSalt =  StringUtilities.ConvertToBase64("DontUseThisInProduction"),
                CryptoSecret =  StringUtilities.GenerateAesSecret(),
                AccessTokenExpiryMinutes = 30,
                RefreshTokenExpiryDays = 3
            });

            services.AddSingleton(new TwilioConfig
            {
                ApiToken = "123456789",
                AccountSid = "123456789",
                AppNumber = "123456789"
            });

            services.AddSingleton(new QuartzConfig
            {
                AccountBalanceRefreshJobCronExpression = "0 */5 * * * ?"
            });
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddScheme<JwtBearerOptions, JwtMiddleware>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = "selah-api",
                    ValidAudience = "selah-api",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(testJwtSecret)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };
            });

            services.AddHttpClient<IPlaidHttpService, PlaidHttpService>(config =>
            {
                config.BaseAddress = new Uri("https://sandbox.plaid.com/");
                config.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
            });
        });
    }
}