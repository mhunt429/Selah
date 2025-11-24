using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using Quartz;
using Domain.Configuration;
using Infrastructure.RecurringJobs;
using Infrastructure.Services;
using Infrastructure.Services.Interfaces;

namespace WebApi.Extensions;

public static class DependencyInjection
{
    public static void AddDependencies(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment env)
    {
        services.RegisterRepositories(configuration, env)
            .AddValidators()
            .AddApplicationServices()
            .AddHttpClients(configuration)
            .RegisterQuartz(configuration, env)
            .AddChannelServices()
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

    public static IServiceCollection RegisterQuartz(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment env)
    {
        if (env.EnvironmentName == "IntegrationTests") return services;

        QuartzConfig quartzConfig = configuration.GetSection("QuartzConfig").Get<QuartzConfig>();
        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("RecurringAccountBalanceUpdateJob");
            q.AddJob<ConnectorDataSyncRecurringJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("RecurringAccountBalanceUpdateJob-startup-trigger")
                .StartNow()
                .WithSimpleSchedule(x => x.WithRepeatCount(0))
            );

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("RecurringAccountBalanceUpdateJob-daily-trigger")
                .WithCronSchedule(quartzConfig.AccountBalanceRefreshJobCronExpression)
            );
        });

        services.AddQuartzHostedService(options => { options.WaitForJobsToComplete = true; });

        return services;
    }
}