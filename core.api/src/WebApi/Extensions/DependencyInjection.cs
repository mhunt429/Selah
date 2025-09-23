using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Quartz;
using MassTransit;
using Domain.Configuration;
using Infrastructure.RecurringJobs;
using Infrastructure.Services;
using Infrastructure.Services.Interfaces;

namespace WebApi.Extensions;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterRepositories(configuration)
            .AddValidators()
            .AddApplicationServices()
            .AddHttpClients(configuration)
            .RegisterQuartz(configuration)
            .RegisterRabbitMq(configuration)
            ;
    }

    private static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        PlaidConfig plaidConfig = configuration.GetSection("PlaidConfig").Get<PlaidConfig>();

        services.AddHttpClient<IPlaidHttpService, PlaidHttpService>(config =>
        {
            config.BaseAddress = new Uri(plaidConfig.BaseUrl);
            config.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
        });

        return services;
    }

    public static IServiceCollection RegisterQuartz(this IServiceCollection services, IConfiguration configuration)
    {
        
        QuartzConfig quartzConfig = configuration.GetSection("QuartzConfig").Get<QuartzConfig>();
        if (quartzConfig == null)
        {
            throw new ArgumentNullException(nameof(quartzConfig));
        }
        
        services.AddQuartz(q =>
        {
            // Use Microsoft DI job factory
            q.UseMicrosoftDependencyInjectionJobFactory();

            // Register job + trigger
            var jobKey = new JobKey("RecurringAccountBalanceUpdateJob");
            q.AddJob<RecurringAccountBalanceUpdateJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("RecurringAccountBalanceUpdateJob-startup-trigger")
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithRepeatCount(0)) // run only once
            );
            
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("RecurringAccountBalanceUpdateJob-daily-trigger")
                .WithCronSchedule(quartzConfig.AccountBalanceRefreshJobCronExpression)
            );
        });
 
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
    public static IServiceCollection RegisterRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        RabbitMqConfig rabbitMqConfig = configuration.GetSection("RabbitMQSettings").Get<RabbitMqConfig>();
        if (rabbitMqConfig == null)
        {
            throw new NullReferenceException("RabbitMq configuration is null");
        }

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.SetInMemorySagaRepositoryProvider();
            var assembly = typeof(Program).Assembly;
            x.AddConsumers(assembly);
            x.AddSagaStateMachines(assembly);
            x.AddSagas(assembly);
            x.AddActivities(assembly);
            x.UsingRabbitMq((context, configuration) =>
            {
                configuration.Host(rabbitMqConfig.Host);
                configuration.Host(rabbitMqConfig.Host, hostConfigurator =>
                {
                    hostConfigurator.Username(rabbitMqConfig.UserName);
                    hostConfigurator.Password(rabbitMqConfig.Password);
                    if (rabbitMqConfig.UseSsl)
                    {
                        hostConfigurator.UseSsl(s =>
                        {
                            s.Protocol = SslProtocols.Tls13;
                            s.ServerName = rabbitMqConfig.SslServerName;
                            var clientCertificate = new X509Certificate2(
                                rabbitMqConfig.ClientCertificatePath
                            );
                            s.Certificate = clientCertificate;
                        });
                    }
                });
            });
        });

        return services;
    }
}