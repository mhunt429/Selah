using System.Diagnostics.CodeAnalysis;
using Infrastructure;
using Infrastructure.Services;
using Infrastructure.Services.Connector;
using Infrastructure.Services.Interfaces;

namespace WebApi.Extensions;

[ExcludeFromCodeCoverage]
public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services
            .AddScoped<ITokenService, TokenService>()
            .AddScoped<IPasswordHasherService, PasswordHasherService>()
            .AddScoped<ICryptoService, CryptoService>()
            .AddScoped<IPlaidAccountBalanceImportService, PlaidAccountBalanceImportService>()
            ;

        return services;
    }
}