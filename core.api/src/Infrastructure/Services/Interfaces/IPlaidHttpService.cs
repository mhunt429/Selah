using Domain.Models.Plaid;

namespace Infrastructure.Services.Interfaces;

public interface IPlaidHttpService
{
    Task<PlaidLinkToken?> GetLinkToken(int userId);

    Task<PlaidTokenExchangeResponse?> ExchangePublicToken(int userId, string publicToken);
}