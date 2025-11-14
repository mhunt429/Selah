using Domain.ApiContracts.Identity;
using Domain.Results;
using Infrastructure.Services.Interfaces;
using MediatR;

namespace Application.Identity;

public class RefreshAccessToken
{
    public class Command : RefreshTokenRequest, IRequest<LoginResult>
    {
    }

    public class Handler : IRequestHandler<Command, LoginResult>
    {
        private readonly ITokenService _tokenService;

        public Handler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<LoginResult> Handle(Command command, CancellationToken cancellationToken)
        {
            AccessTokenResponse? rsp = await _tokenService.RefreshToken(command.RefreshToken);
            if (rsp is null) return new LoginResult(false, null);

            return new LoginResult(true, rsp);
        }
    }
}