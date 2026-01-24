using Domain.ApiContracts.Identity;

namespace Domain.Results;

public enum LoginStatus
{
    Success,
    Failed,
}

public record LoginResult(LoginStatus Status, AccessTokenResponse? AccessTokenResponse);