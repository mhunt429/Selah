using System.Diagnostics;
using Domain.Events;
using Domain.Models.Entities.Mailbox;
using Domain.Models.Plaid;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.Services.Connector;

public class ConnectorEventService(IPlaidAccountBalanceImportService accountBalanceImportService, IUserMailboxRepository mailboxRepository,
    IAccountConnectorRepository connectorRepository, IPlaidTransactionImportService txImportService ): IConnectorEventService
{
    private static readonly ActivitySource ActivitySource = new("cortado-webapi");
    public async Task ProcessEventAsync(ConnectorDataSyncEvent @event)
    {
        using (var _ = ActivitySource.StartActivity(@event.EventType.ToString()))
        {
            if ( @event.Error != null &&
                 @event.Error.ErrorCode == ErrorCodes.LoginRequired)
            {
                await HandleReauthNotification(@event);
                return;
            }

            switch (@event.EventType)
            {
                case EventType.BalanceImport:
                {
                    await accountBalanceImportService.ImportAccountBalancesAsync(@event);
                    break;
                }
                case EventType.TransactionImport:
                {
                    await txImportService.ImportTransactionsAsync(@event);
                    break;
                }
                case EventType.RecurringTransactionImport:
                {
                    await txImportService.ImportRecurringTransactionsAsync(@event);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// If the 3rd party connector becomes disconnected, we want to let the user know
    /// and not process any future messages until they go through the connector update flow
    /// </summary>
    /// <param name="event"></param>
    private async Task HandleReauthNotification(ConnectorDataSyncEvent @event)
    {
        var connectorRecord = await connectorRepository.GetConnectorRecordByIdAndUser(@event.UserId, @event.ConnectorId);
        if (connectorRecord != null)
        {
            var entityToSave = new UserMailboxEntity
            {
                UserId =  connectorRecord.UserId,   
                AppLastChangedBy = -1,
                MessageKey = $"InstitutionAuthRequired",
                MessageBody = @$"Your {connectorRecord.InstitutionName} connection requires you to re-authenticate. 
                    You can reconnect Cortado to ${connectorRecord.InstitutionName} 
                    through this link <a>https://localhost:4200/connector/{connectorRecord.Id}/update</a>.",
                ActionType = "Error"
            };

            await connectorRepository.LockRecordWhenAuthenticationIsRequired(@event.ConnectorId, @event.UserId);
            
            await mailboxRepository.InsertMessage(entityToSave);
        }
    }
}