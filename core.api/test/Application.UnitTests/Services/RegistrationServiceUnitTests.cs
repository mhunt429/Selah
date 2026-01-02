using Application.Services;
using Domain.ApiContracts.AccountRegistration;
using Domain.ApiContracts.Identity;
using Domain.Models;
using Domain.Models.Entities.ApplicationUser;
using Domain.Models.Entities.UserAccount;
using AwesomeAssertions;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.UnitTests.Services;

public class RegistrationServiceUnitTests
{
    private readonly Mock<IRegistrationRepository> _registrationRepository = new();
    private readonly Mock<ICryptoService> _cryptoService = new();
    private readonly Mock<IPasswordHasherService> _passwordHasherService = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly Mock<ILogger<RegistrationService>> _logger = new();
    private readonly Mock<IValidator<AccountRegistrationRequest>> _validatorMock = new();

    private RegistrationService _service;

    public RegistrationServiceUnitTests()
    {
        _registrationRepository
            .Setup(x => x.RegisterAccount( It.IsAny<ApplicationUserEntity>()))
            .ReturnsAsync((1, 2));

        _validatorMock = new Mock<IValidator<AccountRegistrationRequest>>();

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<AccountRegistrationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _service = new RegistrationService(_registrationRepository.Object, _cryptoService.Object,
            _passwordHasherService.Object, _tokenService.Object, _logger.Object, _validatorMock.Object);

        _tokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<bool>()))
            .ReturnsAsync(new AccessTokenResponse
            {
                AccessToken = "token",
                RefreshToken = "refreshToken",
                AccessTokenExpiration = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeMilliseconds(),
                RefreshTokenExpiration = DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeMilliseconds(),
            });
    }
    
    [Fact]
    public async Task Register_ShouldReturnAccessToken()
    {
        var request = new AccountRegistrationRequest()
        {
            FirstName = "Hingle",
            LastName = "McCringleberry",
            Email = "testing123@test.com",
            Password = "AStrongPassword!42",
            PasswordConfirmation = "AStrongPassword!42",
        };

        var result = await _service.RegisterAccount(request);
        result.Should().NotBeNull();
        result.data.Should().NotBeNull();
        result.status.Should().Be(ResultStatus.Success);

        result.data?.AccessToken.Should().Be("token");
        result.data?.RefreshToken.Should().Be("refreshToken");
        result.data?.AccessTokenExpiration.Should().BeGreaterThan(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        result.data?.RefreshTokenExpiration.Should().BeGreaterThan(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    }
}