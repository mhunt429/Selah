using AwesomeAssertions;
using Infrastructure.Services;

namespace Infrastructure.UnitTests.Services;

public class PasswordHasherServiceTests
{
    private readonly PasswordHasherService _passwordHasherService;

    public PasswordHasherServiceTests()
    {
        _passwordHasherService = new PasswordHasherService();
    }

    [Fact]
    public void HashPassword_ShouldReturnCorrectHash()
    {
        string password = "password";

        string passwordHash = _passwordHasherService.HashPassword(password);

        passwordHash.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("password", "Z1xee3FXKGzl3Anq9Pd1wvPN44XQPnfkyUB/Q/b3VdhJGse6jlGghXCAgcJQeSfG", true)]
    [InlineData("password", "password", false)]
    public void VerifyHashedPassword_Validate(string password, string passwordHash, bool expectedResult)
    {
        bool validPassword = _passwordHasherService.VerifyPassword(password, passwordHash);
        validPassword.Should().Be(expectedResult);
    }
}