using System.Threading.Channels;
using Domain.Events;
using Domain.Models;
using Domain.Models.Entities.AccountConnector;
using Domain.Models.Plaid;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;

namespace Application.Services;

public class ConnectorService(
    IPlaidHttpService plaidHttpService,
    IAccountConnectorRepository accountConnectorRepository,
    ICryptoService cryptoService,
    ChannelWriter<ConnectorDataSyncEvent> publisher)
{
    public async Task<ApiResponseResult<PlaidLinkToken>> GetLinkToken(int userId, int? connectorId = null,
        bool forUpdate = false)
    {
        string? accessToken = null;

        if (forUpdate && connectorId != null)
        {
            var connectorRecord =
                await accountConnectorRepository.GetConnectorRecordByIdAndUser(userId, connectorId.Value);
            if (connectorRecord != null)
            {
                accessToken = cryptoService.Decrypt(connectorRecord.EncryptedAccessToken);
            }
        }

        return await plaidHttpService.GetLinkToken(userId, accessToken);
    }

    public async Task ExchangeToken(TokenExchangeHttpRequest request)
    {
        var plaidTokenExchangeResponse =
            await plaidHttpService.ExchangePublicToken(request.UserId,
                request.PublicToken);

        if (plaidTokenExchangeResponse.status == ResultStatus.Failed || plaidTokenExchangeResponse.data == null)
            return;

        var dataToSave = new AccountConnectorEntity
        {
            AppLastChangedBy = request.UserId,
            UserId = request.UserId,
            InstitutionId = request.InstitutionId,
            InstitutionName = request.InstitutionName,
            DateConnected = DateTime.UtcNow,
            EncryptedAccessToken = cryptoService.Encrypt(plaidTokenExchangeResponse.data.AccessToken),
            TransactionSyncCursor = null,
            ExternalEventId = plaidTokenExchangeResponse.data.ItemId,
            LastSyncDate = DateTimeOffset.UtcNow,
            NextSyncDate = DateTimeOffset.MaxValue
        };

        await accountConnectorRepository.InsertAccountConnectorRecord(dataToSave);

        var syncEvent = new ConnectorDataSyncEvent
        {
            AccessToken = dataToSave.EncryptedAccessToken,
            UserId = request.UserId,
            ConnectorId = dataToSave.Id,
            EventType = EventType.BalanceImport
        };

        await publisher.WriteAsync(syncEvent);
    }
}