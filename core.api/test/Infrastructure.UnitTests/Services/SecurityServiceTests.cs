using FluentAssertions;
using HashidsNet;
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
            HashIdSalt = "",
            AccessTokenExpiryMinutes = 60,
            RefreshTokenExpiryDays = 30,
        };

        IHashids hashids = new Hashids("secretSalt", minHashLength: 24);

        _cryptoService = new CryptoService(securityConfig, hashids, passwordHasherServiceMock.Object);
    }

    [Fact]
    public void Encrypt_ShouldCreateTwoDifferentEncryptedValues_ForTheSameInput()
    {
        string string1 = "This is super secret!!!";
        string string2 = "This is super secret!!!";

        string encryptedString1 = _cryptoService.Encrypt(string1);
        string encryptedString2 = _cryptoService.Encrypt(string2);

        encryptedString1.Should().NotBe(encryptedString2);
    }

    [Fact]
    public void Decrypt_ShouldDecryptTheInput()
    {
        string plaintext = "test";
        string encryptedString = _cryptoService.Encrypt(plaintext);
        string decryptedString = _cryptoService.Decrypt(encryptedString);
        decryptedString.Should().Be("test");
    }

    [Fact]
    public void DecodeHashId_ShouldReturnPlainTextId()
    {
        long plainId = 1;

        string hashId = _cryptoService.EncodeHashId(plainId);

        long expectedHashId = _cryptoService.DecodeHashId(hashId);
        expectedHashId.Should().Be(1);
    }
}