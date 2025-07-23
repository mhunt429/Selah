using MediatR;
using Domain.Models;
using Domain.Models.Entities.AccountConnector;
using Domain.Models.Plaid;
using Infrastructure.Repository;
using Infrastructure.Services.Interfaces;

namespace Application.AccountConnector;

public class ExchangeLinkToken
{
    public class Command : TokenExchangeHttpRequest, IRequest<ApiResponseResult<Unit>>
    {
    }

    public class Handler : IRequestHandler<Command, ApiResponseResult<Unit>>
    {
        private readonly IAccountConnectorRepository _accountConnectorRepository;
        private readonly ICryptoService _cryptoService;
        private readonly IPlaidHttpService _plaidHttpService;

        public Handler(IAccountConnectorRepository accountConnectorRepository, ICryptoService cryptoService,
            IPlaidHttpService plaidHttpService)
        {
            _accountConnectorRepository = accountConnectorRepository;
            _cryptoService = cryptoService;
            _plaidHttpService = plaidHttpService;
        }

        public async Task<ApiResponseResult<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            PlaidTokenExchangeResponse? plaidTokenExchangeResponse =
                await _plaidHttpService.ExchangePublicToken(request.UserId,
                    request.PublicToken);

            if (plaidTokenExchangeResponse == null)
            {
                return new ApiResponseResult<Unit>(status: ResultStatus.Failed, message: "", data: new Unit());
            }
            //If we get a token back from Plaid, save the record into the account_connector table

            AccountConnectorEntity dataToSave = new AccountConnectorEntity
            {
                AppLastChangedBy = request.UserId,
                UserId = request.UserId,
                InstitutionId = request.InstitutionId,
                InstitutionName = request.InstitutionName,
                DateConnected = DateTime.UtcNow,
                EncryptedAccessToken = _cryptoService.Encrypt(plaidTokenExchangeResponse.AccessToken),
                TransactionSyncCursor = "",
                ExternalEventId = plaidTokenExchangeResponse.ItemId,
                OriginalInsert = DateTimeOffset.UtcNow,
            };

            await _accountConnectorRepository.InsertAccountConnectorRecord(dataToSave);

            return new ApiResponseResult<Unit>(status: ResultStatus.Success, message: "", data: new Unit());
        }
    }
}