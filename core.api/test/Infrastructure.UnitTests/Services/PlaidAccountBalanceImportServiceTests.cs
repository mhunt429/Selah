using System.Threading.Channels;
using Domain.Events;
using Domain.Models;
using Domain.Models.Entities.FinancialAccount;
using Domain.Models.Plaid;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Connector;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Infrastructure.UnitTests.Services;

public class PlaidAccountBalanceImportServiceTests
{
    private readonly Mock<ICryptoService> _mockCryptoService;
    private readonly Mock<IFinancialAccountRepository> _mockFinancialAccountRepository;
    private readonly Mock<IPlaidHttpService> _mockPlaidHttpService;
    private readonly Mock<IAccountConnectorRepository> _mockAccountConnectorRepository;
    private readonly PlaidAccountBalanceImportService _service;
    private readonly Mock<ChannelWriter<ConnectorDataSyncEvent>> mockChannelWriter;

    public PlaidAccountBalanceImportServiceTests()
    {
        _mockCryptoService = new Mock<ICryptoService>();
        _mockFinancialAccountRepository = new Mock<IFinancialAccountRepository>();
        _mockPlaidHttpService = new Mock<IPlaidHttpService>();
        var mockLogger = new Mock<ILogger<PlaidAccountBalanceImportService>>();
        _mockAccountConnectorRepository = new Mock<IAccountConnectorRepository>();
        mockChannelWriter = new Mock<ChannelWriter<ConnectorDataSyncEvent>>();
        
        _mockFinancialAccountRepository
            .Setup(x => x.GetAccountsAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<FinancialAccountEntity>());


        _service = new PlaidAccountBalanceImportService(
            _mockCryptoService.Object,
            _mockFinancialAccountRepository.Object,
            _mockPlaidHttpService.Object,
            mockLogger.Object,
            _mockAccountConnectorRepository.Object,
            mockChannelWriter.Object);
    }

    [Fact]
    public async Task ImportAccountBalancesAsync_WhenSuccessful_ImportsNewAccounts()
    {
        // Arrange
        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            ConnectorId = 1,
            AccessToken = new byte[] { 1, 2, 3 },
            EventType = EventType.BalanceImport
        };

        var decryptedToken = "decrypted-access-token";
        var balanceResponse = new PlaidBalanceApiResponse
        {
            Accounts = new List<PlaidAccountBalance>
            {
                new PlaidAccountBalance
                {
                    AccountId = "account-1",
                    Name = "Test Account",
                    OfficialName = "Test Official Name",
                    Subtype = "checking",
                    Mask = "1234",
                    Balance = new Balances
                    {
                        Current = 1000.50m,
                        Available = 950.00m,
                        IsoCurrencyCode = "USD"
                    }
                }
            }
        };

        _mockCryptoService
            .Setup(x => x.Decrypt(syncEvent.AccessToken))
            .Returns(decryptedToken);

        _mockPlaidHttpService
            .Setup(x => x.GeAccountBalance(decryptedToken))
            .ReturnsAsync(new ApiResponseResult<PlaidBalanceApiResponse>(
                ResultStatus.Success,
                "Success",
                balanceResponse));

      
        _mockFinancialAccountRepository
            .Setup(x => x.ImportFinancialAccountsAsync(It.IsAny<IEnumerable<FinancialAccountEntity>>()))
            .Returns(Task.CompletedTask);

        _mockAccountConnectorRepository
            .Setup(x => x.UpdateConnectionSyncCursor(
                syncEvent.ConnectorId,
                syncEvent.UserId,
                It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ImportAccountBalancesAsync(syncEvent);

        // Assert
        _mockCryptoService.Verify(x => x.Decrypt(syncEvent.AccessToken), Times.Once);
        _mockPlaidHttpService.Verify(x => x.GeAccountBalance(decryptedToken), Times.Once);
        _mockFinancialAccountRepository.Verify(
            x => x.ImportFinancialAccountsAsync(It.Is<IEnumerable<FinancialAccountEntity>>(accounts =>
                accounts.Any(a => a.ExternalId == "account-1" && a.CurrentBalance == 1000.50m))),
            Times.Once);
        _mockAccountConnectorRepository.Verify(
            x => x.UpdateAccountSyncTimes(
                syncEvent.ConnectorId,
                syncEvent.UserId,
                It.IsAny<DateTimeOffset>()),
            Times.Once);
    }

    [Fact]
    public async Task ImportAccountBalancesAsync_WhenAccountsExist_UpdatesBalances()
    {
        // Arrange
        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            ConnectorId = 1,
            AccessToken = new byte[] { 1, 2, 3 },
            EventType = EventType.BalanceImport
        };

        var decryptedToken = "decrypted-access-token";
        var existingAccount = new FinancialAccountEntity
        {
            AppLastChangedBy = 1,
            Id = 1,
            ExternalId = "account-1",
            UserId = syncEvent.UserId,
            CurrentBalance = 500.00m,
            ConnectorId = 0,
            AccountMask = "1234",
            DisplayName = "My Checking 1",
            Subtype = "checking"
        };

        var balanceResponse = new PlaidBalanceApiResponse
        {
            Accounts = new List<PlaidAccountBalance>
            {
                new PlaidAccountBalance
                {
                    AccountId = "account-1",
                    Name = "Test Account",
                    Balance = new Balances
                    {
                        Current = 1000.50m,
                        Available = 950.00m,
                        IsoCurrencyCode = "USD"
                    }
                }
            }
        };

        _mockCryptoService
            .Setup(x => x.Decrypt(syncEvent.AccessToken))
            .Returns(decryptedToken);

        _mockPlaidHttpService
            .Setup(x => x.GeAccountBalance(decryptedToken))
            .ReturnsAsync(new ApiResponseResult<PlaidBalanceApiResponse>(
                ResultStatus.Success,
                "Success",
                balanceResponse));

        _mockFinancialAccountRepository
            .Setup(x => x.GetAccountsAsync(syncEvent.UserId))
            .ReturnsAsync(new List<FinancialAccountEntity> { existingAccount });

        _mockFinancialAccountRepository
            .Setup(x => x.UpdateAccount(It.IsAny<FinancialAccountEntity>()))
            .ReturnsAsync(true);

        _mockFinancialAccountRepository
            .Setup(x => x.InsertBalanceHistory(It.IsAny<AccountBalanceHistoryEntity>()))
            .Returns(Task.CompletedTask);

        _mockAccountConnectorRepository
            .Setup(x => x.UpdateConnectionSyncCursor(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ImportAccountBalancesAsync(syncEvent);

        // Assert
        _mockFinancialAccountRepository.Verify(
            x => x.UpdateAccount(It.Is<FinancialAccountEntity>(a =>
                a.ExternalId == "account-1" && a.CurrentBalance == 1000.50m)),
            Times.Once);
        _mockFinancialAccountRepository.Verify(
            x => x.InsertBalanceHistory(It.Is<AccountBalanceHistoryEntity>(h =>
                h.FinancialAccountId == existingAccount.Id && h.CurrentBalance == 1000.50m)),
            Times.Once);
    }

    [Fact]
    public async Task ImportAccountBalancesAsync_WhenApiFails_LogsErrorAndReturns()
    {
        // Arrange
        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            ConnectorId = 1,
            AccessToken = new byte[] { 1, 2, 3 },
            EventType = EventType.BalanceImport
        };

        var decryptedToken = "decrypted-access-token";
        var errorMessage = "API Error";

        _mockCryptoService
            .Setup(x => x.Decrypt(syncEvent.AccessToken))
            .Returns(decryptedToken);

        _mockPlaidHttpService
            .Setup(x => x.GeAccountBalance(decryptedToken))
            .ReturnsAsync(new ApiResponseResult<PlaidBalanceApiResponse>(
                ResultStatus.Failed,
                errorMessage,
                null));

        // Act
        await _service.ImportAccountBalancesAsync(syncEvent);

        // Assert
        _mockPlaidHttpService.Verify(x => x.GeAccountBalance(decryptedToken), Times.Once);
        _mockFinancialAccountRepository.Verify(
            x => x.GetAccountsAsync(It.IsAny<int>()),
            Times.Never);
        _mockAccountConnectorRepository.Verify(
            x => x.UpdateConnectionSyncCursor(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ImportAccountBalancesAsync_WhenBalanceDataIsNull_DoesNotImport()
    {
        // Arrange
        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            ConnectorId = 1,
            AccessToken = new byte[] { 1, 2, 3 },
            EventType = EventType.BalanceImport
        };

        var decryptedToken = "decrypted-access-token";

        _mockCryptoService
            .Setup(x => x.Decrypt(syncEvent.AccessToken))
            .Returns(decryptedToken);

        _mockPlaidHttpService
            .Setup(x => x.GeAccountBalance(decryptedToken))
            .ReturnsAsync(new ApiResponseResult<PlaidBalanceApiResponse>(
                ResultStatus.Success,
                "Success",
                null));

        // Act
        await _service.ImportAccountBalancesAsync(syncEvent);

        // Assert
        _mockFinancialAccountRepository.Verify(
            x => x.GetAccountsAsync(It.IsAny<int>()),
            Times.Once);
        _mockAccountConnectorRepository.Verify(
            x => x.UpdateConnectionSyncCursor(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ImportAccountBalancesAsync_CanHandleAndParseError()
    {
        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            ConnectorId = 1,
            AccessToken = new byte[] { 1, 2, 3 },
            EventType = EventType.BalanceImport
        };

        var responseBody = new PlaidApiErrorResponse
        {
            ErrorCode = "ITEM_LOGIN_REQUIRED",
            ErrorMessage = "the login details of this item have changed"
        };

        _mockPlaidHttpService
            .Setup(x => x.GeAccountBalance(It.IsAny<string>()))
            .ReturnsAsync(new ApiResponseResult<PlaidBalanceApiResponse>(
                ResultStatus.Failed,
                JsonSerializer.Serialize(responseBody),
                null));

        await _service.ImportAccountBalancesAsync(syncEvent);

        mockChannelWriter.Verify(
            x => x.WriteAsync(It.Is<ConnectorDataSyncEvent>(e =>
                e.UserId == 123 &&
                e.ConnectorId == 1 &&
                e.EventType == EventType.BalanceImport &&
                e.Error != null &&
                e.Error.ErrorCode == responseBody.ErrorCode &&
                e.Error.ErrorMessage == responseBody.ErrorMessage
            )),
            Times.Once
        );
    }
}