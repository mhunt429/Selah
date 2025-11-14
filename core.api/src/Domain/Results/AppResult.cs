using Domain.ApiContracts.Identity;

namespace Domain.Results;

public record Success();

public record Failed(string Message);


public record LoginResult(bool Success, AccessTokenResponse? AccessTokenResponse );