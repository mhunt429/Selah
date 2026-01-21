using System.Diagnostics.CodeAnalysis;
using Application.Services;
using Infrastructure;
using Infrastructure.Services;
using Infrastructure.Services.Connector;
using Infrastructure.Services.Interfaces;

namespace WebApi.Extensions;


public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services
            .AddScoped<ITokenService, TokenService>()
            .AddScoped<IPasswordHasherService, PasswordHasherService>()
            .AddScoped<ICryptoService, CryptoService>()
            .AddScoped<IPlaidAccountBalanceImportService, PlaidAccountBalanceImportService>()
            .AddScoped<IPlaidTransactionImportService, PlaidTransactionImportService>()
            .AddScoped<IConnectorEventService, ConnectorEventService>()
            .AddScoped<AppUserService>()
            .AddScoped<ConnectorService>()
            .AddScoped<IdentityService>()
            .AddScoped<RegistrationService>()
            .AddScoped<BankingService>()
            .AddScoped<SupportService>()
            .AddScoped<UserMailboxService>()
            ;

        return services;
    }
}