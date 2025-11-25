using Infrastructure.Metrics;
using Infrastructure.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Workers;

public class ActiveSessionsWorkerService(
    ILogger<ActiveSessionsWorkerService> logger,
    IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation($"{nameof(ActiveSessionsWorkerService)} is starting");
                using var scope = scopeFactory.CreateScope();
                var sessionRepository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

                var activeSessions = await sessionRepository.GetActiveSessions();

                logger.LogInformation("Current active sessions: {ActiveSessions}", activeSessions);

                MetricsRegistry.ActiveSessions = activeSessions;

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception in ActiveSessionsWorkerService");
            }
        }
    }
}