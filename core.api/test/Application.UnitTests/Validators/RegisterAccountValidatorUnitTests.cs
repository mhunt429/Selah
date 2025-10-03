using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using Application.Validators;
using Domain.ApiContracts.AccountRegistration;
using Domain.Models.Entities.ApplicationUser;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;

namespace Application.UnitTests.Validators;

public class RegisterAccountValidatorUnitTests
{
    private readonly Mock<IApplicationUserRepository> _userRepositoryMock = new();
    private readonly Mock<ICryptoService> _cryptoServiceMock = new();

    private readonly RegisterAccountValidator _validator;

    public RegisterAccountValidatorUnitTests()
    {
        _validator = new RegisterAccountValidator(_userRepositoryMock.Object, _cryptoServiceMock.Object);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("testing123@", "@assword!")]
    public async Task Validator_ShouldValidateAgainstInvalidData(string email, string password)
    {
        var data = new AccountRegistrationRequest
        {
            FirstName = "",
            LastName = "",
            Email = email,
            Password = password,
        };

        _userRepositoryMock.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(new ApplicationUserEntity
        {
            EncryptedEmail = default,
            Password = default,
            EncryptedName = default,
            EncryptedPhone = default,
            EmailHash = default,
            AppLastChangedBy = default
        });

        var result = await _validator.TestValidateAsync(data);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.ShouldHaveValidationErrorFor(command => command.FirstName);
        result.ShouldHaveValidationErrorFor(command => command.LastName);
        result.ShouldHaveValidationErrorFor(command => command.Email);
        result.ShouldHaveValidationErrorFor(command => command.Password);
    }

    [Fact]
    public async Task Validator_ShouldAllowValidData()
    {
        var data = new AccountRegistrationRequest
        {
            FirstName = "Hingle",
            LastName = "McCringleberry",
            Email = "testing123@test.com",
            Password = "AStrongPassword!42",
            PasswordConfirmation = "AStrongPassword!42",
        };

        _userRepositoryMock.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync((ApplicationUserEntity)null);

        var result = await _validator.TestValidateAsync(data);
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}