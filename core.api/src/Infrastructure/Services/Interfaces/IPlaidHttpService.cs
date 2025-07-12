using Domain.Models.Plaid;

namespace Infrastructure.Services.Interfaces;

public interface IPlaidHttpService
{
    Task<PlaidLinkToken?> GetLinkToken(Guid userId);

    Task<PlaidTokenExchangeResponse?> ExchangePublicToken(Guid userId, string publicToken);
}