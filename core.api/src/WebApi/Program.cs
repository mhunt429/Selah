using System.Diagnostics.CodeAnalysis;
using System.Text;
using Akka.Actor;
using Akka.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Infrastructure;
using WebApi.Extensions;
using WebApi.Middleware;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using Application.ApplicationUser;
using Domain.Constants;
using Infrastructure.Services.Workers;
using Npgsql;
using OpenTelemetry.Metrics;
using Amazon.SQS;
//using Infrastructure.BackgroundWorkers;

namespace WebApi;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        DotNetEnv.Env.Load("../.env");
        ConfigureServices(builder);

        var app = builder.Build();
        ConfigureApp(app, builder.Configuration);

        app.Run();
    }

    private static IServiceCollection ConfigureServices(WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        //Because of the automatic dependency injection with mediatr and we have all of that in the Application Project, we just need to pass in a single IRequest instance
        builder.Services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssemblyContaining(typeof(GetUserById.Query)));

        builder.Services.AddSingleton<IDbConnectionFactory>(provider =>
        {
            var connectionString = configuration.GetValue<string>("SelahDbConnectionString");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ApplicationException("No connection string configured.");
            }

            return new SelahDbConnectionFactory(connectionString);
        });

        builder.Services.AddConfiguration(configuration);
        builder.Services.AddDependencies(configuration);

        builder.Services.AddOpenApi();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
                policy.WithOrigins(ClientUrls.Localhost, ClientUrls.Production)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });

        ConfigureAuthentication(builder.Services, configuration);

        builder.Services.AddAuthorization();
        builder.Services.AddControllers();

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddNpgsql()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter();
            }).WithMetrics(x =>
            {
                x.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter()
                    .AddMeter("Microsoft.AspNetCore.Hosting",
                        "Microsoft.AspNetCore.Server.Kestrel",
                        "selah-webapi")
                    .AddView("request-duration",
                        new ExplicitBucketHistogramConfiguration
                        {
                            Boundaries = new[]
                                { 0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 }
                        }).AddPrometheusExporter();
                ;
            });

        builder.Services.AddMetrics();

        builder.Logging.ClearProviders();
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
            logging.ParseStateValues = true;
            logging.AddOtlpExporter();
        });
     
        builder.Services.AddAWSService<IAmazonSQS>();
        
        var bootstrap = BootstrapSetup.Create();
        var di = DependencyResolverSetup.Create(builder.Services.BuildServiceProvider());
       

        //builder.Services.AddHostedService<AmazonSqsListener>();
        builder.Services.AddHostedService<ActiveSessionsWorkerService>();

        return builder.Services;
    }

    private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        string jwtSecret = configuration["SecurityConfig:JwtSecret"];

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddScheme<JwtBearerOptions, JwtMiddleware>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = "selah-api",
                ValidAudience = "selah-api",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
            };
        });
    }

    private static void ConfigureApp(WebApplication app, IConfiguration configuration)
    {
        if (app.Environment.IsDevelopment())
        {
            IdentityModelEventSource.ShowPII = true;
            app.MapOpenApi();

            app.UseSwaggerUI(options =>
                options.SwaggerEndpoint("/openapi/v1.json", "Selah.AppHost.WebAPI")
            );
            app.UseReDoc(options => { options.SpecUrl = "/openapi/v1.json"; });

            app.MapScalarApiReference();
        }

        app.UseOpenTelemetryPrometheusScrapingEndpoint();

        app.UseRouting();

        app.UseMiddleware<RequestLoggingMiddleware>();

        app.UseCors();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<ExceptionHandler>();
        app.MapControllers();
    }
}