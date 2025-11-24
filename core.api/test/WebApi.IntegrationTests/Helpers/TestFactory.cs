using System.Text;
using Domain.Configuration;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using WebApi.Middleware;

namespace WebApi.IntegrationTests.Helpers;

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

            services.AddSingleton(new SecurityConfig
            {
                JwtSecret = "DontUseThisInProduction",
                HashIdSalt = "DontUseThisInProduction",
                CryptoSecret = "DontUseThisInProduction",
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("DontUseThisInProduction")),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };
            });
        });
    }
}