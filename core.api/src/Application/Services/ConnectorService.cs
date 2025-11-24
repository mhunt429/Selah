using Domain.Models;
using Domain.Models.Entities.AccountConnector;
using Domain.Models.Plaid;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;

namespace Application.Services;

public class ConnectorService
{
    private readonly IPlaidHttpService _plaidHttpService;
    private readonly IAccountConnectorRepository _accountConnectorRepository;
    private readonly ICryptoService _cryptoService;

    public ConnectorService(IPlaidHttpService plaidHttpService, IAccountConnectorRepository accountConnectorRepository,
        ICryptoService cryptoService)
    {
        _plaidHttpService = plaidHttpService;
        _accountConnectorRepository = accountConnectorRepository;
        _cryptoService = cryptoService;
    }

    public async Task<ApiResponseResult<PlaidLinkToken>> GetLinkToken(int userId)
    {
        return await _plaidHttpService.GetLinkToken(userId);
    }

    public async Task ExchangeToken(TokenExchangeHttpRequest request)
    {
        var plaidTokenExchangeResponse =
            await _plaidHttpService.ExchangePublicToken(request.UserId,
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
            EncryptedAccessToken = _cryptoService.Encrypt(plaidTokenExchangeResponse.data.AccessToken),
            TransactionSyncCursor = "",
            ExternalEventId = plaidTokenExchangeResponse.data.ItemId
        };

        await _accountConnectorRepository.InsertAccountConnectorRecord(dataToSave);
    }
}