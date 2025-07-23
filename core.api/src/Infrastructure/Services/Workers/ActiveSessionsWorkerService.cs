using Infrastructure.Metrics;
using Infrastructure.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class ActiveSessionsWorkerService : BackgroundService
{
    private readonly ILogger<ActiveSessionsWorkerService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public ActiveSessionsWorkerService(
        ILogger<ActiveSessionsWorkerService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation($"{nameof(ActiveSessionsWorkerService)} is starting");
                using var scope = _scopeFactory.CreateScope();
                var sessionRepository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();
                
                var activeSessions = await sessionRepository.GetActiveSessions();

                _logger.LogInformation("Current active sessions: {ActiveSessions}", activeSessions);
                
                MetricsRegistry.ActiveSessions = activeSessions;
                
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in ActiveSessionsWorkerService");
            }
        }
    }
}