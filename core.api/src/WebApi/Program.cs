using System.Text;
using Domain.Constants;
using Infrastructure;
using Infrastructure.Services.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using WebApi.Extensions;
using WebApi.Middleware;


namespace WebApi;

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

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Services.AddSingleton<IDbConnectionFactory>(provider =>
        {
            var connectionString = configuration.GetValue<string>("SelahDbConnectionString");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ApplicationException("No connection string configured.");
            }

            return new SelahDbConnectionFactory(connectionString);
        });


        builder.Services.AddConfiguration(configuration, builder.Environment);
        builder.Services.AddDependencies(configuration, builder.Environment);

        builder.Services.AddOpenApi();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
                policy.WithOrigins(Constants.Localhost, Constants.Production)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });

        ConfigureAuthentication(builder.Services, configuration, builder.Environment);

        builder.Services.AddAuthorization();
        builder.Services.AddControllers();

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource("selah-webapi")
                    .AddAspNetCoreInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
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
        
        // Filter out EF Core database command logs before adding OpenTelemetry
        builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
        
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
            logging.ParseStateValues = true;
            logging.AddOtlpExporter();
        });

        builder.Services.AddHostedService<ActiveSessionsWorkerService>();

        builder.Services.AddHealthChecks();

        builder.Services.RegisterRateLimiter(builder.Environment);
    }

    private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment env)
    {
        if (env.EnvironmentName == "IntegrationTests") return;

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
        app.UseForwardedHeaders();
        app.UseRateLimiter();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<ExceptionHandler>();
        app.MapControllers();
        app.MapHealthChecks("/hc");
    }
}