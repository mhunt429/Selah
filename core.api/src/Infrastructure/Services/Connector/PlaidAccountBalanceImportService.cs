using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Connector;

public class PlaidAccountBalanceImportService(
    IAccountConnectorRepository accountConnectorRepository,
    IPlaidHttpService plaidHttpService,
    IFinancialAccountRepository financialAccountRepository,
    ILogger<PlaidAccountBalanceImportService> logger,
    ICryptoService cryptoService)
{
    private readonly IAccountConnectorRepository _accountConnectorRepository = accountConnectorRepository;
    private readonly IFinancialAccountRepository _financialAccountRepository = financialAccountRepository;
    private readonly ILogger<PlaidAccountBalanceImportService> _logger = logger;
    private readonly IPlaidHttpService _plaidHttpService = plaidHttpService;
    private readonly ICryptoService _cryptoService = cryptoService;


    public async Task ImportAccountBalancesAsync()
    {
       
    }
}