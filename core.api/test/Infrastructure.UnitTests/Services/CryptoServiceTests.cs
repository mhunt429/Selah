using Domain.Configuration;
using Domain.Shared;
using Infrastructure.Services;
using Infrastructure.Services.Interfaces;
using Moq;
using Xunit;

namespace Infrastructure.UnitTests.Services;

public class CryptoServiceTests
{
    private readonly Mock<IPasswordHasherService> _mockPasswordHasherService;
    private readonly SecurityConfig _securityConfig;
    private readonly CryptoService _cryptoService;

    public CryptoServiceTests()
    {
        _mockPasswordHasherService = new Mock<IPasswordHasherService>();
        
        // Generate a valid base64 key for AES (must be 16, 24, or 32 bytes)
        var keyBytes = new byte[32];
        System.Security.Cryptography.RandomNumberGenerator.Fill(keyBytes);
        var base64Key = Convert.ToBase64String(keyBytes);

        _securityConfig = new SecurityConfig
        {
            CryptoSecret = base64Key,
            JwtSecret = StringUtilities.GenerateSecret(64),
            AccessTokenExpiryMinutes = 60,
            RefreshTokenExpiryDays = 30
        };

        _cryptoService = new CryptoService(_securityConfig, _mockPasswordHasherService.Object);
    }

    [Fact]
    public void Encrypt_WhenGivenPlainText_ReturnsEncryptedBytes()
    {
        // Arrange
        var plainText = "test-encryption-string";

        // Act
        var encrypted = _cryptoService.Encrypt(plainText);

        // Assert
        Assert.NotNull(encrypted);
        Assert.NotEmpty(encrypted);
        Assert.True(encrypted.Length > 16); // Should contain IV (16 bytes) + ciphertext
    }

    [Fact]
    public void Decrypt_WhenGivenEncryptedBytes_ReturnsOriginalPlainText()
    {
        // Arrange
        var plainText = "test-decryption-string";
        var encrypted = _cryptoService.Encrypt(plainText);

        // Act
        var decrypted = _cryptoService.Decrypt(encrypted);

        // Assert
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void EncryptAndDecrypt_WithDifferentTexts_WorksCorrectly()
    {
        // Arrange
        var testCases = new[]
        {
            "simple",
            "text with spaces",
            "text-with-special-chars!@#$%^&*()",
            "Unicode: 测试 🚀",
            "",
            "very long text " + new string('x', 1000)
        };

        foreach (var plainText in testCases)
        {
            // Act
            var encrypted = _cryptoService.Encrypt(plainText);
            var decrypted = _cryptoService.Decrypt(encrypted);

            // Assert
            Assert.Equal(plainText, decrypted);
        }
    }

    [Fact]
    public void Decrypt_WhenDataIsTooShort_ThrowsArgumentException()
    {
        // Arrange
        var shortData = new byte[10]; // Less than 16 bytes (IV size)

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _cryptoService.Decrypt(shortData));
    }

    [Fact]
    public void Decrypt_WhenDataIsExactly16Bytes_ThrowsArgumentException()
    {
        // Arrange
        var exactly16Bytes = new byte[16]; // Exactly IV size, no ciphertext

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _cryptoService.Decrypt(exactly16Bytes));
    }

    [Fact]
    public void HashValue_WhenGivenPlainText_ReturnsHashString()
    {
        // Arrange
        var plainText = "test-hash-value";

        // Act
        var hash = _cryptoService.HashValue(plainText);

        // Assert
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
        Assert.Equal(64, hash.Length); // SHA256 produces 64 hex characters
    }

    [Fact]
    public void HashValue_WithSameInput_ReturnsSameHash()
    {
        // Arrange
        var plainText = "consistent-hash-test";

        // Act
        var hash1 = _cryptoService.HashValue(plainText);
        var hash2 = _cryptoService.HashValue(plainText);

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void HashValue_WithDifferentInputs_ReturnsDifferentHashes()
    {
        // Arrange
        var text1 = "test1";
        var text2 = "test2";

        // Act
        var hash1 = _cryptoService.HashValue(text1);
        var hash2 = _cryptoService.HashValue(text2);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void HashPassword_DelegatesToPasswordHasherService()
    {
        // Arrange
        var password = "test-password";
        var expectedHash = "hashed-password";

        _mockPasswordHasherService
            .Setup(x => x.HashPassword(password))
            .Returns(expectedHash);

        // Act
        var result = _cryptoService.HashPassword(password);

        // Assert
        Assert.Equal(expectedHash, result);
        _mockPasswordHasherService.Verify(x => x.HashPassword(password), Times.Once);
    }

    [Fact]
    public void VerifyPassword_DelegatesToPasswordHasherService()
    {
        // Arrange
        var password = "test-password";
        var passwordHash = "hashed-password";
        var expectedResult = true;

        _mockPasswordHasherService
            .Setup(x => x.VerifyPassword(password, passwordHash))
            .Returns(expectedResult);

        // Act
        var result = _cryptoService.VerifyPassword(password, passwordHash);

        // Assert
        Assert.Equal(expectedResult, result);
        _mockPasswordHasherService.Verify(x => x.VerifyPassword(password, passwordHash), Times.Once);
    }
}

