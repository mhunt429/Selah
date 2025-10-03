using Domain.Models;
using Domain.Models.Entities.AccountConnector;
using Domain.Models.Plaid;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;
using MediatR;

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
            var plaidTokenExchangeResponse =
                await _plaidHttpService.ExchangePublicToken(request.UserId,
                    request.PublicToken);

            if (plaidTokenExchangeResponse.status == ResultStatus.Failed || plaidTokenExchangeResponse.data == null)
                return new ApiResponseResult<Unit>(ResultStatus.Failed, "", new Unit());
            //If we get a token back from Plaid, save the record into the account_connector table

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

            return new ApiResponseResult<Unit>(ResultStatus.Success, "", new Unit());
        }
    }
}