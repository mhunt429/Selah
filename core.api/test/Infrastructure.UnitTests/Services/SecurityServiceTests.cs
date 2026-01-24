using AwesomeAssertions;
using Moq;
using Domain.Configuration;
using Infrastructure.Services;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.UnitTests.Services;

public class SecurityServiceTests
{
    private readonly ICryptoService _cryptoService;

    public SecurityServiceTests()
    {
        Mock<IPasswordHasherService> passwordHasherServiceMock = new();

        var securityConfig = new SecurityConfig
        {
            CryptoSecret = "QXIwjylLOdmMMfhjC1nv601gyxU+EABjSvf1iADe0Qw=",
            JwtSecret = "",
            AccessTokenExpiryMinutes = 60,
            RefreshTokenExpiryDays = 30,
        };


        _cryptoService = new CryptoService(securityConfig, passwordHasherServiceMock.Object);
    }

    [Fact]
    public void Encrypt_ShouldCreateTwoDifferentEncryptedValues_ForTheSameInput()
    {
        string string1 = "abc123";
        string string2 = "abc123";

        byte[] encryptedString1 = _cryptoService.Encrypt(string1);
        byte[] encryptedString2 = _cryptoService.Encrypt(string2);

        var hex1 = Convert.ToHexString(encryptedString1);
        var hex2 = Convert.ToHexString(encryptedString2);

        hex1.Should().NotBe(hex2);
    }

    [Fact]
    public void Decrypt_ShouldDecryptTheInput()
    {
        string plaintext = "test";
        byte[] encryptedString = _cryptoService.Encrypt(plaintext);
        string decryptedString = _cryptoService.Decrypt(encryptedString);
        decryptedString.Should().Be("test");
    }
    
}