using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Webhooks.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterMessageQueuing(builder.Configuration);

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

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();

app.UseHttpsRedirection();

app.MapControllers();
app.Run();