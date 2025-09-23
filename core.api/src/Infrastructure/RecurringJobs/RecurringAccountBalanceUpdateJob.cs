using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.RecurringJobs;

public class RecurringAccountBalanceUpdateJob : IJob
{
    private readonly ILogger<RecurringAccountBalanceUpdateJob> _logger;
    private readonly IPlaidHttpService _plaidHttpService;

    public RecurringAccountBalanceUpdateJob(
        ILogger<RecurringAccountBalanceUpdateJob> logger,
        IPlaidHttpService plaidHttpService)
    {
        _logger = logger;
        _plaidHttpService = plaidHttpService;
    }

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Recurring account balance update");
        return Task.CompletedTask;
    }
}