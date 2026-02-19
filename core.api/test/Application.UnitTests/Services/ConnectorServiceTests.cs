using System.Threading.Channels;
using Domain.Models;
using Domain.Models.Entities.AccountConnector;
using Domain.Models.Plaid;
using Application.Services;
using AwesomeAssertions;
using Domain.ApiContracts.Connector;
using Domain.Configuration;
using Domain.Events;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;
using Moq;
using NetTopologySuite.Mathematics;
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

        var plaidConfig = new PlaidConfig
        {
            ClientId = "ABC123",
            ClientSecret = "ABC123",
            BaseUrl = "https://localhost:5001",
            MaxDaysRequested = 0
        };


        _service = new ConnectorService(
            _mockPlaidHttpService.Object,
            _mockAccountConnectorRepository.Object,
            _mockCryptoService.Object,
            _publisher.Object,
            plaidConfig);
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
            .Setup(x => x.GetLinkToken(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetLinkToken(userId);

        // Assert
        Assert.Equal(expectedResponse, result);
        _mockPlaidHttpService.Verify(x => x.GetLinkToken(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetLinkToken_ShouldRequestAccessTokenForUpdateMode()
    {
        var userId = 123;
        _mockAccountConnectorRepository.Setup(x => x.GetConnectorRecordByIdAndUser(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new AccountConnectorEntity
            {
                InstitutionId = "123",
                InstitutionName = "Jane Street",
                DateConnected = DateTimeOffset.Now,
                EncryptedAccessToken = new byte[]
                {
                },
                TransactionSyncCursor = "abc123",
                AppLastChangedBy = 1,
                Id = 1
            });

        _mockCryptoService.Setup(x => x.Decrypt(It.IsAny<byte[]>())).Returns("my token");

        await _service.GetLinkToken(userId, connectorId: 1, forUpdate: true);

        _mockPlaidHttpService.Verify(x => x.GetLinkToken(123, "my token"), Times.Once);
    }

    [Fact]
    public async Task GetLinkToken_DoesNotSendAccessTokenWithNullConnector()
    {
        var userId = 123;
        _mockAccountConnectorRepository.Setup(x => x.GetConnectorRecordByIdAndUser(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((AccountConnectorEntity?)null);


        await _service.GetLinkToken(userId, connectorId: 1, forUpdate: true);

        _mockCryptoService.Verify(x => x.Decrypt(It.IsAny<byte[]>()), Times.Never);
        _mockPlaidHttpService.Verify(x => x.GetLinkToken(123, null), Times.Once);
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

    [Fact]
    public async Task UpdateConnection_ShouldReturnFalseWhenConnectorRecordDoesNotExist()
    {
        _mockAccountConnectorRepository.Setup(x => x.GetConnectorRecordByIdAndUser(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((AccountConnectorEntity?)null);
        var result = await _service.UpdateConnection(1, 1);
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, true)]
    public async Task UpdateConnection_ShouldReturnFalseWhenPlaidReturnsFailureOrSuccessWithErrorMessage(
        bool returnsFailure, bool returnsError, bool returnsNoData)
    {
        _mockAccountConnectorRepository.Setup(x => x.GetConnectorRecordByIdAndUser(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new AccountConnectorEntity
            {
                InstitutionId = "123",
                InstitutionName = "Test Bank",
                DateConnected = default,
                EncryptedAccessToken = new byte[]
                {
                },
                TransactionSyncCursor = null,
                AppLastChangedBy = 0
            });
        _mockCryptoService.Setup(x => x.Decrypt(It.IsAny<byte[]>()))
            .Returns("secret");

        if (returnsFailure)
        {
            _mockPlaidHttpService.Setup(x => x.GetItem(It.IsAny<BasePlaidRequest>()))
                .ReturnsAsync(new ApiResponseResult<GetItemResponse>(ResultStatus.Failed, null, null));
        }

        if (returnsNoData)
        {
            _mockPlaidHttpService.Setup(x => x.GetItem(It.IsAny<BasePlaidRequest>()))
                .ReturnsAsync(new ApiResponseResult<GetItemResponse>(ResultStatus.Success, null, null));
        }

        if (returnsError)
        {
            _mockPlaidHttpService.Setup(x => x.GetItem(It.IsAny<BasePlaidRequest>()))
                .ReturnsAsync(new ApiResponseResult<GetItemResponse>(ResultStatus.Success, null, new GetItemResponse
                {
                    Item = new Item
                    {
                        Error = new PlaidApiErrorResponse
                        {
                            ErrorMessage = "Bad Things Happened"
                        }
                    }
                }));
        }

        var result = await _service.UpdateConnection(1, 1);
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateConnection_ShouldReturnDatabaseReturnValue(bool retVal)
    {
        _mockAccountConnectorRepository.Setup(x => x.GetConnectorRecordByIdAndUser(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new AccountConnectorEntity
            {
                InstitutionId = "123",
                InstitutionName = "Test Bank",
                DateConnected = default,
                EncryptedAccessToken = new byte[]
                {
                },
                TransactionSyncCursor = null,
                AppLastChangedBy = 0
            });
        _mockCryptoService.Setup(x => x.Decrypt(It.IsAny<byte[]>()))
            .Returns("secret");

        _mockPlaidHttpService.Setup(x => x.GetItem(It.IsAny<BasePlaidRequest>()))
            .ReturnsAsync(new ApiResponseResult<GetItemResponse>(ResultStatus.Success, null, new GetItemResponse
            {
                Item = new Item
                {
                    Error = null
                }
            }));

        _mockAccountConnectorRepository.Setup(x => x.RemoveConnectionSyncLock(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(retVal);
        var result = await _service.UpdateConnection(1, 1);
        result.Should().Be(retVal);
    }
}