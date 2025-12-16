using Domain.Events;
using Domain.Models;
using Domain.Models.Entities.FinancialAccount;
using Domain.Models.Plaid;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Connector;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Infrastructure.UnitTests.Services;

public class PlaidAccountBalanceImportServiceTests
{
    private readonly Mock<ICryptoService> _mockCryptoService;
    private readonly Mock<IFinancialAccountRepository> _mockFinancialAccountRepository;
    private readonly Mock<IPlaidHttpService> _mockPlaidHttpService;
    private readonly Mock<ILogger<PlaidAccountBalanceImportService>> _mockLogger;
    private readonly Mock<IAccountConnectorRepository> _mockAccountConnectorRepository;
    private readonly PlaidAccountBalanceImportService _service;

    public PlaidAccountBalanceImportServiceTests()
    {
        _mockCryptoService = new Mock<ICryptoService>();
        _mockFinancialAccountRepository = new Mock<IFinancialAccountRepository>();
        _mockPlaidHttpService = new Mock<IPlaidHttpService>();
        _mockLogger = new Mock<ILogger<PlaidAccountBalanceImportService>>();
        _mockAccountConnectorRepository = new Mock<IAccountConnectorRepository>();

        _service = new PlaidAccountBalanceImportService(
            _mockCryptoService.Object,
            _mockFinancialAccountRepository.Object,
            _mockPlaidHttpService.Object,
            _mockLogger.Object,
            _mockAccountConnectorRepository.Object);
    }

    [Fact]
    public async Task ImportAccountBalancesAsync_WhenSuccessful_ImportsNewAccounts()
    {
        // Arrange
        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            DataSyncId = 1,
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
            .Setup(x => x.GetAccountsAsync(syncEvent.UserId))
            .ReturnsAsync(new List<FinancialAccountEntity>());

        _mockFinancialAccountRepository
            .Setup(x => x.ImportFinancialAccountsAsync(It.IsAny<IEnumerable<FinancialAccountEntity>>()))
            .Returns(Task.CompletedTask);

        _mockAccountConnectorRepository
            .Setup(x => x.UpdateConnectionSync(
                syncEvent.DataSyncId,
                syncEvent.UserId,
                It.IsAny<DateTimeOffset>()))
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
            x => x.UpdateConnectionSync(
                syncEvent.DataSyncId,
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
            DataSyncId = 1,
            ConnectorId = 1,
            AccessToken = new byte[] { 1, 2, 3 },
            EventType = EventType.BalanceImport
        };

        var decryptedToken = "decrypted-access-token";
        var existingAccount = new FinancialAccountEntity
        {
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
            .Setup(x => x.UpdateConnectionSync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTimeOffset>()))
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
            DataSyncId = 1,
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
            x => x.UpdateConnectionSync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTimeOffset>()),
            Times.Never);
    }

    [Fact]
    public async Task ImportAccountBalancesAsync_WhenBalanceDataIsNull_DoesNotImport()
    {
        // Arrange
        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            DataSyncId = 1,
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
            x => x.UpdateConnectionSync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTimeOffset>()),
            Times.Never);
    }
}

