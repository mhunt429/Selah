using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Domain.ApiContracts.AccountRegistration;
using Domain.ApiContracts.Identity;
using Domain.Models;
using Domain.Models.Entities.ApplicationUser;
using Domain.Models.Entities.UserAccount;
using Infrastructure.Repository;
using Infrastructure.Services.Interfaces;

namespace Application.Registration;

public class RegisterAccount
{
    public class Command : AccountRegistrationRequest, IRequest<ApiResponseResult<AccessTokenResponse>>
    {
    }

    public class Handler : IRequestHandler<Command, ApiResponseResult<AccessTokenResponse>>
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly ICryptoService _cryptoService;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<Handler> _logger;
        private readonly IValidator<AccountRegistrationRequest> _accountRegistrationRequestValidator;

        public Handler(IRegistrationRepository registrationRepository, ICryptoService cryptoService,
            IPasswordHasherService passwordHasherService, ITokenService tokenService, ILogger<Handler> logger,
            IValidator<AccountRegistrationRequest> accountRegistrationRequestValidator)
        {
            _registrationRepository = registrationRepository;
            _cryptoService = cryptoService;
            _passwordHasherService = passwordHasherService;
            _tokenService = tokenService;
            _logger = logger;
            _accountRegistrationRequestValidator = accountRegistrationRequestValidator;
        }

        public async Task<ApiResponseResult<AccessTokenResponse>> Handle(Command command,
            CancellationToken cancellationToken)
        {
            var validationResult = await _accountRegistrationRequestValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return new ApiResponseResult<AccessTokenResponse>(status: ResultStatus.Failed, data: null,
                    message: default, errors: validationResult.Errors.Select(x => x.ErrorMessage));
            }

            //Since this is the entry point of the account creation, initialize 2 unique ids for the account and user
            Guid accountId = Guid.CreateVersion7(DateTime.UtcNow);
            Guid userId = Guid.CreateVersion7(DateTime.UtcNow);

            UserAccountEntity userAccountEntity = MapRequestToUserAccount(command, accountId, userId);
            ApplicationUserEntity applicationUserEntity = MapRequestToUser(command, accountId, userId);

            await _registrationRepository.RegisterAccount(userAccountEntity, applicationUserEntity);

            AccessTokenResponse accessTokenResponse = _tokenService.GenerateAccessToken(userId);

            _logger.LogInformation("User with id {id} was successfully created", userId);
            return new ApiResponseResult<AccessTokenResponse>(status: ResultStatus.Success, data: accessTokenResponse,
                message: default, errors: default);
        }

        private UserAccountEntity MapRequestToUserAccount(AccountRegistrationRequest request, Guid accountId,
            Guid userId)
        {
            return new UserAccountEntity
            {
                AppLastChangedBy = userId,
                Id = accountId,
                CreatedOn = DateTime.UtcNow,
                AccountName = request.AccountName,
                OriginalInsert = DateTimeOffset.UtcNow,
            };
        }

        private ApplicationUserEntity MapRequestToUser(AccountRegistrationRequest request, Guid accountId, Guid userId)
        {
            return new ApplicationUserEntity
            {
                AppLastChangedBy = userId,
                AccountId = accountId,
                Id = userId,
                Password = _passwordHasherService.HashPassword(request.Password),
                EncryptedEmail = _cryptoService.Encrypt(request.Email),
                EncryptedName = _cryptoService.Encrypt($"{request.FirstName}|{request.LastName}"),
                EncryptedPhone = _cryptoService.Encrypt(request.PhoneNumber),
                PhoneVerified = false,
                EmailVerified = false,
                EmailHash = _cryptoService.HashValue(request.Email),
                CreatedDate = DateTimeOffset.UtcNow,
                OriginalInsert = DateTimeOffset.UtcNow,
            };
        }
    }
}