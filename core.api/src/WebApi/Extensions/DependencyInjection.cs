using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Hangfire;
using Hangfire.PostgreSql;
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
            .RegisterHangfire(configuration)
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

    public static IServiceCollection RegisterHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetValue<string>("SelahDbConnectionString");
        services.AddHangfire(x =>
            x.UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString))
        );
        services.AddHangfireServer();
        services.AddTransient<RecurringAccountBalanceUpdateJob>();
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