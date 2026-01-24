using System.Threading.Channels;
using Domain.Models;
using Domain.Models.Entities.AccountConnector;
using Domain.Models.Plaid;
using Application.Services;
using Domain.Events;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;
using Moq;
using Xunit;

namespace Application.UnitTests.Services;

public class ConnectorServiceTests
{
    private readonly Mock<IPlaidHttpService> _mockPlaidHttpService;
    private readonly Mock<IAccountConnectorRepository> _mockAccountConnectorRepository;
    private readonly Mock<ICryptoService> _mockCryptoService;
    private readonly ConnectorService _service;
    private readonly Mock<ChannelWriter<ConnectorDataSyncEvent>> _publisher;

    public ConnectorServiceTests()
    {
        _mockPlaidHttpService = new Mock<IPlaidHttpService>();
        _mockAccountConnectorRepository = new Mock<IAccountConnectorRepository>();
        _mockCryptoService = new Mock<ICryptoService>();
        _publisher = new Mock<ChannelWriter<ConnectorDataSyncEvent>>();

        _service = new ConnectorService(
            _mockPlaidHttpService.Object,
            _mockAccountConnectorRepository.Object,
            _mockCryptoService.Object,
            _publisher.Object);
    }

    [Fact]
    public async Task GetLinkToken_WhenCalled_DelegatesToPlaidHttpService()
    {
        // Arrange
        var userId = 123;
        var expectedResponse = new ApiResponseResult<PlaidLinkToken>(
            ResultStatus.Success,
            "Success",
            new PlaidLinkToken { LinkToken = "test-link-token" });

        _mockPlaidHttpService
            .Setup(x => x.GetLinkToken(It.IsAny<int>(), It.IsAny<bool>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLinkToken(userId);

        // Assert
        Assert.Equal(expectedResponse, result);
        _mockPlaidHttpService.Verify(x => x.GetLinkToken(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task ExchangeToken_WhenSuccessful_SavesAccountConnector()
    {
        // Arrange
        var request = new TokenExchangeHttpRequest
        {
            UserId = 123,
            PublicToken = "public-token",
            InstitutionId = "inst-123",
            InstitutionName = "Test Bank"
        };

        var accessToken = "access-token";
        var itemId = "item-123";
        var encryptedToken = new byte[] { 1, 2, 3 };

        var tokenExchangeResponse = new ApiResponseResult<PlaidTokenExchangeResponse>(
            ResultStatus.Success,
            "Success",
            new PlaidTokenExchangeResponse
            {
                AccessToken = accessToken,
                ItemId = itemId
            });

        _mockPlaidHttpService
            .Setup(x => x.ExchangePublicToken(request.UserId, request.PublicToken))
            .ReturnsAsync(tokenExchangeResponse);

        _mockCryptoService
            .Setup(x => x.Encrypt(accessToken))
            .Returns(encryptedToken);

        _mockAccountConnectorRepository
            .Setup(x => x.InsertAccountConnectorRecord(It.IsAny<AccountConnectorEntity>()))
            .ReturnsAsync(1);

        // Act
        await _service.ExchangeToken(request);

        // Assert
        _mockPlaidHttpService.Verify(x => x.ExchangePublicToken(request.UserId, request.PublicToken), Times.Once);
        _mockCryptoService.Verify(x => x.Encrypt(accessToken), Times.Once);
        _mockAccountConnectorRepository.Verify(
            x => x.InsertAccountConnectorRecord(It.Is<AccountConnectorEntity>(entity =>
                entity.UserId == request.UserId &&
                entity.InstitutionId == request.InstitutionId &&
                entity.InstitutionName == request.InstitutionName &&
                entity.EncryptedAccessToken == encryptedToken &&
                entity.ExternalEventId == itemId)),
            Times.Once);
    }

    [Fact]
    public async Task ExchangeToken_WhenApiFails_DoesNotSaveConnector()
    {
        // Arrange
        var request = new TokenExchangeHttpRequest
        {
            UserId = 123,
            PublicToken = "public-token",
            InstitutionId = "inst-123",
            InstitutionName = "Test Bank"
        };

        var tokenExchangeResponse = new ApiResponseResult<PlaidTokenExchangeResponse>(
            ResultStatus.Failed,
            "Error",
            null);

        _mockPlaidHttpService
            .Setup(x => x.ExchangePublicToken(request.UserId, request.PublicToken))
            .ReturnsAsync(tokenExchangeResponse);

        // Act
        await _service.ExchangeToken(request);

        // Assert
        _mockCryptoService.Verify(x => x.Encrypt(It.IsAny<string>()), Times.Never);
        _mockAccountConnectorRepository.Verify(
            x => x.InsertAccountConnectorRecord(It.IsAny<AccountConnectorEntity>()),
            Times.Never);
    }

    [Fact]
    public async Task ExchangeToken_WhenDataIsNull_DoesNotSaveConnector()
    {
        // Arrange
        var request = new TokenExchangeHttpRequest
        {
            UserId = 123,
            PublicToken = "public-token",
            InstitutionId = "inst-123",
            InstitutionName = "Test Bank"
        };

        var tokenExchangeResponse = new ApiResponseResult<PlaidTokenExchangeResponse>(
            ResultStatus.Success,
            "Success",
            null);

        _mockPlaidHttpService
            .Setup(x => x.ExchangePublicToken(request.UserId, request.PublicToken))
            .ReturnsAsync(tokenExchangeResponse);

        // Act
        await _service.ExchangeToken(request);

        // Assert
        _mockCryptoService.Verify(x => x.Encrypt(It.IsAny<string>()), Times.Never);
        _mockAccountConnectorRepository.Verify(
            x => x.InsertAccountConnectorRecord(It.IsAny<AccountConnectorEntity>()),
            Times.Never);
    }
}

