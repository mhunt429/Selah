using Domain.ApiContracts.AccountRegistration;
using Domain.ApiContracts.Identity;
using Domain.Models;
using Domain.Models.Entities.ApplicationUser;
using Domain.Models.Entities.UserAccount;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class RegistrationService
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly ICryptoService _cryptoService;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<RegistrationService> _logger;
    private readonly IValidator<AccountRegistrationRequest> _accountRegistrationRequestValidator;

    public RegistrationService(IRegistrationRepository registrationRepository, ICryptoService cryptoService,
        IPasswordHasherService passwordHasherService, ITokenService tokenService, ILogger<RegistrationService> logger,
        IValidator<AccountRegistrationRequest> accountRegistrationRequestValidator)
    {
        _registrationRepository = registrationRepository;
        _cryptoService = cryptoService;
        _passwordHasherService = passwordHasherService;
        _tokenService = tokenService;
        _logger = logger;
        _accountRegistrationRequestValidator = accountRegistrationRequestValidator;
    }

    public async Task<ApiResponseResult<AccessTokenResponse>> RegisterAccount(AccountRegistrationRequest request)
    {
        ValidationResult? validationResult = await _accountRegistrationRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return new ApiResponseResult<AccessTokenResponse>(status: ResultStatus.Failed, data: null,
                message: string.Join(",", validationResult.Errors.Select(x => x.ErrorMessage)));
        }
        
        ApplicationUserEntity applicationUserEntity = MapRequestToUser(request);

        (int, int) registrationResult =
            await _registrationRepository.RegisterAccount(applicationUserEntity);

        // Ensure the user is fully committed before generating token
        // This prevents foreign key constraint violations when tests run together
        var userId = registrationResult.Item2;
        
        AccessTokenResponse accessTokenResponse = await _tokenService.GenerateAccessToken(userId);

        _logger.LogInformation("User with id {id} was successfully created", registrationResult.Item2);
        return new ApiResponseResult<AccessTokenResponse>(status: ResultStatus.Success, data: accessTokenResponse,
            message: default);
    }

    /*
     * Since this is on account creation, there obviously isn't a user id set thus breaking
     * the FK constraint on the app_user table. The ID -1 is a placeholder for the cortado-api default user
     */

    private ApplicationUserEntity MapRequestToUser(AccountRegistrationRequest request)
    {
        return new ApplicationUserEntity
        {
            AppLastChangedBy = -1,
            Password = _passwordHasherService.HashPassword(request.Password),
            EncryptedEmail = _cryptoService.Encrypt(request.Email),
            EncryptedName = _cryptoService.Encrypt($"{request.FirstName}|{request.LastName}"),
            EncryptedPhone = _cryptoService.Encrypt(request.PhoneNumber),
            PhoneVerified = false,
            EmailVerified = false,
            EmailHash = _cryptoService.HashValue(request.Email),
            CreatedDate = DateTimeOffset.UtcNow,
            UserAccount = new UserAccountEntity
            {
                AppLastChangedBy = -1, 
                AccountName = request.AccountName,
                CreatedOn = DateTimeOffset.UtcNow,
            }
        };
    }
}