using System.Diagnostics.CodeAnalysis;
using HashidsNet;
using Domain.Configuration;

namespace WebApi.Extensions;


public static class ConfigurationExtensions
{
    public static void AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        AwsConfig awsConfig = configuration.GetSection("AwsConfig").Get<AwsConfig>();
       
        services.AddSingleton(awsConfig);

        PlaidConfig plaidConfig = configuration.GetSection("PlaidConfig").Get<PlaidConfig>();

        services.AddSingleton(plaidConfig);

        SecurityConfig securityConfig = configuration.GetSection("SecurityConfig").Get<SecurityConfig>();
        services.AddSingleton(securityConfig);
        
        services.AddSingleton<IHashids>(_ => new Hashids(securityConfig.HashIdSalt, minHashLength: 24));
        
        TwilioConfig twilioConfig = configuration.GetSection("TwilioConfig").Get<TwilioConfig>();

        QuartzConfig quartzConfig = configuration.GetSection("QuartzConfig").Get<QuartzConfig>();

        services.AddSingleton(quartzConfig);
    }
}