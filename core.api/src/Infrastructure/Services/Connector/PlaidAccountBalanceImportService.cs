using System.Threading.Channels;
using Domain.Events;
using Domain.Models;
using Domain.Models.Entities.FinancialAccount;
using Domain.Models.Plaid;
using Infrastructure.Extensions;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Connector;

public class PlaidAccountBalanceImportService(
    ICryptoService cryptoService,
    IFinancialAccountRepository financialAccountRepository,
    IPlaidHttpService plaidHttpService,
    ILogger<PlaidAccountBalanceImportService> logger,
    IAccountConnectorRepository accountConnectorRepository,
    ChannelWriter<ConnectorDataSyncEvent> publisher): IPlaidAccountBalanceImportService
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

            syncEvent.ParseErrorResponse(balancesResponse.message);
            
            await publisher.WriteAsync(syncEvent);
            
            return;
        }

        PlaidBalanceApiResponse? balanceData = balancesResponse.data;

        var existingAccounts = (await financialAccountRepository.GetAccountsAsync(syncEvent.UserId))
            .ToDictionary(a => a.ExternalId);


        if (balanceData != null)
        {
            if (!existingAccounts.Any())
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
                    AccountMask = a.Mask,
                    LastApiSyncTime = DateTimeOffset.UtcNow,
                    IsExternalApiImport = true
                });

                await financialAccountRepository.ImportFinancialAccountsAsync(dbObjectsToSave);
            }
            // Just update existing balances
            else
            {
                foreach (var account in balanceData.Accounts)
                {
                    if (existingAccounts.TryGetValue(account.AccountId, out var existing))
                    {
                        existing.CurrentBalance = account.Balance!.Current;
                        existing.LastApiSyncTime = DateTimeOffset.UtcNow;
                        
                        await financialAccountRepository.UpdateAccount(existing);
                        
                        var balanceHistory = new AccountBalanceHistoryEntity
                        {
                            UserId = syncEvent.UserId,
                            FinancialAccountId = existing.Id,
                            CurrentBalance = account.Balance!.Current,
                            CreatedAt = DateTimeOffset.UtcNow
                        };
                        await financialAccountRepository.InsertBalanceHistory(balanceHistory);
                    }
                }
            }


            await accountConnectorRepository.UpdateConnectionSync(syncEvent.ConnectorId,
                syncEvent.UserId,
                DateTimeOffset.UtcNow.AddDays(3));
        }
    }
}