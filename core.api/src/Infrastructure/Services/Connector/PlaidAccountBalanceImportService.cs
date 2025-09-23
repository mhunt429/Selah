using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Connector;

public class PlaidAccountBalanceImportService
{
    private readonly IAccountConnectorRepository _accountConnectorRepository;
    private readonly IFinancialAccountRepository _financialAccountRepository;
    private readonly ILogger<PlaidAccountBalanceImportService> _logger;
    private readonly IPlaidHttpService _plaidHttpService;

    public PlaidAccountBalanceImportService(IAccountConnectorRepository accountConnectorRepository,
        IPlaidHttpService plaidHttpService,
        IFinancialAccountRepository financialAccountRepository, ILogger<PlaidAccountBalanceImportService> logger)
    {
        _accountConnectorRepository = accountConnectorRepository;
        _plaidHttpService = plaidHttpService;
        _financialAccountRepository = financialAccountRepository;
        _logger = logger;
    }

    //This services gets called via a job 
    public async Task ImportAccountBalancesAsync(int userId)
    {
    }
}