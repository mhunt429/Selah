using Domain.Events;
using Domain.Models;
using Domain.Models.Entities.FinancialAccount;
using Domain.Models.Plaid;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Connector;

public class PlaidAccountBalanceImportService(
    ICryptoService cryptoService,
    IFinancialAccountRepository financialAccountRepository,
    IPlaidHttpService plaidHttpService,
    ILogger<PlaidAccountBalanceImportService> logger,
    IAccountConnectorRepository accountConnectorRepository)
{
    public async Task ImportAccountBalancesAsync(ConnectorDataSyncEvent syncEvent)
    {
        var accessToken = cryptoService.Decrypt(syncEvent.AccessToken);
        ApiResponseResult<PlaidBalanceApiResponse> balancesResponse =
            await plaidHttpService.GeAccountBalance(accessToken);

        if (balancesResponse.status == ResultStatus.Failed)
        {
            logger.LogError("Account Balance Update failed for user {UserId} with error {ErrorMsg}", syncEvent.UserId,
                balancesResponse.message);
            return;
        }

        PlaidBalanceApiResponse? balanceData = balancesResponse.data;
        if (balanceData != null)
        {
            var dbObjectsToSave = balanceData.Accounts.Select(a => new FinancialAccountEntity
            {
                DisplayName = a.Name,
                OfficialName = a.OfficialName,
                Subtype = a.Subtype,
                ExternalId = a.AccountId,
                ConnectorId = syncEvent.ConnectorId,
                UserId = syncEvent.UserId,
                CurrentBalance = a.Balance!.Current,
                AccountMask = a.Mask
            });

            await financialAccountRepository.ImportFinancialAccountsAsync(dbObjectsToSave);

            await accountConnectorRepository.UpdateConnectionSync(syncEvent.DataSyncId,
                syncEvent.UserId,
                DateTimeOffset.UtcNow.AddDays(3));
        }
    }
}