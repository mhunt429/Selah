using System.Diagnostics.CodeAnalysis;
using Domain.Configuration;

namespace WebApi.Extensions;


[ExcludeFromCodeCoverage(Justification = "Not really necessary since this just handles the startup DI")]
public static class ConfigurationExtensions
{
    public static void AddConfiguration(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        if (environment.EnvironmentName == "IntegrationTests") return;
       
        AwsConfig? awsConfig = configuration.GetSection("AwsConfig").Get<AwsConfig>();

        if(awsConfig == null) throw new Exception("AwsConfig not found");
        
        services.AddSingleton(awsConfig);

        PlaidConfig? plaidConfig = configuration.GetSection("PlaidConfig").Get<PlaidConfig>();
        if (plaidConfig == null) throw new Exception("PlaidConfig not found");

        services.AddSingleton(plaidConfig);

        SecurityConfig? securityConfig = configuration.GetSection("SecurityConfig").Get<SecurityConfig>();
        if (securityConfig == null) throw new Exception("SecurityConfig not found");
        services.AddSingleton(securityConfig);
        
       // TwilioConfig? twilioConfig = configuration.GetSection("TwilioConfig").Get<TwilioConfig>();

        QuartzConfig? quartzConfig = configuration.GetSection("QuartzConfig").Get<QuartzConfig>();

        if (quartzConfig == null) throw new Exception("QuartzConfig not found");
        
        services.AddSingleton(quartzConfig);
    }
}