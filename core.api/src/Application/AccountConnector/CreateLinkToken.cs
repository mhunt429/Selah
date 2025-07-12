using MediatR;
using Domain.Models.Plaid;
using Infrastructure.Services.Interfaces;

namespace Application.AccountConnector;

public class CreateLinkToken
{
    public class Command : IRequest<PlaidLinkToken>
    {
        public Guid UserId { get; set; }
    }

    public class Handler : IRequestHandler<Command, PlaidLinkToken?>
    {
        private readonly IPlaidHttpService _plaidHttpService;

        public Handler(IPlaidHttpService plaidHttpService)
        {
            _plaidHttpService = plaidHttpService;
        }
            
        public async  Task<PlaidLinkToken?> Handle(Command command, CancellationToken cancellationToken)
        {
            return await _plaidHttpService.GetLinkToken(command.UserId);
        }
    }
}

