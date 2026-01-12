using System.Threading.Channels;
using Domain.Events;
using Domain.Models.Entities.AccountConnector;
using Domain.Models.Entities.Mailbox;
using Domain.Models.Plaid;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Connector;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Channels;

public class AccountConnectorEventChannel : BackgroundService
{
    private readonly ChannelReader<ConnectorDataSyncEvent> _reader;
    private readonly IServiceScopeFactory _scopeFactory;

    public AccountConnectorEventChannel(
        ChannelReader<ConnectorDataSyncEvent> reader,
        IServiceScopeFactory scopeFactory)
    {
        _reader = reader;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _reader.WaitToReadAsync(stoppingToken))
        {
            while (_reader.TryRead(out ConnectorDataSyncEvent? connectorDataSyncEvent))
            {
                if (connectorDataSyncEvent.Error != null &&
                    connectorDataSyncEvent.Error.ErrorCode == ErrorCodes.LoginRequired)
                {
                    await HandleReauthNotification(connectorDataSyncEvent);
                    return;
                }

                switch (connectorDataSyncEvent.EventType)
                {
                    case EventType.BalanceImport:
                    {
                        using var scope = _scopeFactory.CreateScope();

                        var importService = scope.ServiceProvider
                            .GetRequiredService<PlaidAccountBalanceImportService>();

                        await importService.ImportAccountBalancesAsync(connectorDataSyncEvent);
                        break;
                    }
                    
                    case EventType.TransactionImport:
                    {
                        break;
                    } 
             
                }
            }
        }
    }

    private async Task HandleReauthNotification(ConnectorDataSyncEvent connectorDataSyncEvent)
    {
        using var scope = _scopeFactory.CreateScope();
        var mailboxRepository = scope.ServiceProvider.GetRequiredService<IUserMailboxRepository>();
        var accountConnectorRepository = scope.ServiceProvider.GetRequiredService<IAccountConnectorRepository>();
        
        AccountConnectorEntity? connnectorRecord = await accountConnectorRepository.GetConnectorSyncRecordByConnectorId(connectorDataSyncEvent.ConnectorId, connectorDataSyncEvent.UserId);
        if (connnectorRecord != null)
        {
            var entityToSave = new UserMailboxEntity
            {
                MessageKey = $"BankReauthRequired",
                MessageBody = @$"Your {connnectorRecord.InstitutionName} connection requires you to re-authenticate. 
You can reconnect Selah to ${connnectorRecord.InstitutionName} 
through this link <a>https://localhost:4200/connector/${connnectorRecord.Id}/update</a>.",
                ActionType = "Error"
            };
            
            await mailboxRepository.InsertMessage(entityToSave);
        }
    }
}