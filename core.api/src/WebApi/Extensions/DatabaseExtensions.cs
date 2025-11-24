using System.Diagnostics.CodeAnalysis;
using Infrastructure;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection services,
        IConfiguration configuration,  IWebHostEnvironment environment)
    {
        var connectionString = environment.EnvironmentName != "IntegrationTests" ? configuration.GetValue<string>("SelahDbConnectionString")
                : "User ID=postgres;Password=postgres;Host=localhost;Port=65432;Database=postgres";

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("SelahDbConnectionString is missing");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<BaseRepository>()
            .AddScoped<IRegistrationRepository, RegistrationRepository>()
            .AddScoped<IApplicationUserRepository, AppUserRepository>()
            .AddScoped<IAccountConnectorRepository, AccountConnectorRepository>()
            .AddScoped<IFinancialAccountRepository, FinancialAccountRepository>()
            .AddScoped<IUserSessionRepository, UserSessionRepository>()
            .AddScoped<TokenRepository>();

        return services;
    }
}