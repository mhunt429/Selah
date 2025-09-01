using FluentValidation;
using Application.Validators;
using Domain.ApiContracts.AccountRegistration;

namespace WebApi.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AccountRegistrationRequest>, RegisterAccountValidator>();

        return services;
    }
}