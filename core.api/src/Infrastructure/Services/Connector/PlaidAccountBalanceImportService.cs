using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.Services.Connector;

public class PlaidAccountBalanceImportService
{
    private readonly IPlaidHttpService _plaidHttpService;
    private readonly IFinancialAccountRepository _financialAccountRepository;

    public PlaidAccountBalanceImportService(IPlaidHttpService plaidHttpService,
        IFinancialAccountRepository financialAccountRepository)
    {
        _plaidHttpService = plaidHttpService;
        _financialAccountRepository = financialAccountRepository;
    }

}