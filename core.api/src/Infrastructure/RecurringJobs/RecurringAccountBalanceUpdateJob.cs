using Hangfire;
using Microsoft.Extensions.Logging;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.RecurringJobs;

[AutomaticRetry(Attempts = 0)]
public class RecurringAccountBalanceUpdateJob
{
    private readonly ILogger<RecurringAccountBalanceUpdateJob> _logger;
    private readonly IPlaidHttpService _plaidHttpService;

    public RecurringAccountBalanceUpdateJob(ILogger<RecurringAccountBalanceUpdateJob> logger, IPlaidHttpService plaidHttpService)
    {
        _logger = logger;
        _plaidHttpService = plaidHttpService;
    }


    public void DoWork()
    {
        _logger.LogInformation("Recurring account balance update");
    }
}