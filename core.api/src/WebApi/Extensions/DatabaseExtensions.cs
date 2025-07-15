using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;

namespace WebApi.Extensions;

[ExcludeFromCodeCoverage]
public static class DatabaseExtensions
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection services,
        IConfiguration configuration)
    {
        string? connectionString = configuration.GetValue<string>("SelahDbConnectionString");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "SelahDbConnectionString is missing");
        }

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<BaseRepository>()
            .AddScoped<IRegistrationRepository, RegistrationRepository>()
            .AddScoped<IApplicationUserRepository, AppUserRepository>()
            .AddScoped<IAccountConnectorRepository, AccountConnectorRepository>()
            .AddScoped<IFinancialAccountRepository, FinancialAccountRepository>()
            .AddScoped<IUserSessionRepository, UserSessionRepository>();

        return services;
    }
}