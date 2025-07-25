using Domain.Models;
using Domain.Models.Plaid;

namespace Infrastructure.Services.Interfaces;

public interface IPlaidHttpService
{
    Task<ApiResponseResult<PlaidLinkToken>> GetLinkToken(int userId);

    Task<ApiResponseResult<PlaidTokenExchangeResponse>> ExchangePublicToken(int userId, string publicToken);

    Task<ApiResponseResult<PlaidBalanceApiResponse>> GeAccountBalance(string accessToken);
}