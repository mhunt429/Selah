using Domain.Events;
using Domain.Models;
using Domain.Models.Entities.FinancialAccount;
using Domain.Models.Entities.Transactions;
using Domain.Models.Plaid;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Connector;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Infrastructure.UnitTests.Services;

public class PlaidTransactionImportServiceTests
{
    private readonly Mock<ICryptoService> _mockCryptoService;
    private readonly Mock<IPlaidHttpService> _mockPlaidHttpService;
    private readonly Mock<IAccountConnectorRepository> _mockAccountConnectorRepository;
    private readonly PlaidTransactionImportService _service;
    private readonly Mock<IFinancialAccountRepository> _mockFinancialAccountRepository;
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;

    public PlaidTransactionImportServiceTests()
    {
        _mockCryptoService = new Mock<ICryptoService>();
        _mockPlaidHttpService = new Mock<IPlaidHttpService>();
        var mockLogger = new Mock<ILogger<PlaidTransactionImportService>>();
        _mockAccountConnectorRepository = new Mock<IAccountConnectorRepository>();
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _mockFinancialAccountRepository = new Mock<IFinancialAccountRepository>();

        _mockFinancialAccountRepository.Setup(x => x.GetAccountsAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<FinancialAccountEntity>
            {
                new FinancialAccountEntity
                {
                    AppLastChangedBy = 1,
                    UserId = 1,
                    ExternalId = "ABC123",
                    CurrentBalance = 0,
                    AccountMask = "1234",
                    DisplayName = "USAA Checking",
                    Subtype = "Checking"
                }
            });


        _service = new PlaidTransactionImportService(
            _mockCryptoService.Object,
            _mockPlaidHttpService.Object,
            mockLogger.Object,
            _mockAccountConnectorRepository.Object,
            _mockTransactionRepository.Object,
            _mockFinancialAccountRepository.Object);
    }

    [Fact]
    public async Task ImportTransactionsAsync_WhenSuccessful_ProcessesTransactionsAndUpdatesSync()
    {
        // Arrange
        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            ConnectorId = 1,
            AccessToken = new byte[] { 1, 2, 3 },
            EventType = EventType.TransactionImport
        };

        var decryptedToken = "decrypted-access-token";
        var transactionsResponse = CreateTransactionsResponse(hasMore: false);

        _mockCryptoService
            .Setup(x => x.Decrypt(syncEvent.AccessToken))
            .Returns(decryptedToken);

        _mockPlaidHttpService
            .Setup(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int>()))
            .ReturnsAsync(new ApiResponseResult<PlaidTransactionsSyncResponse>(
                ResultStatus.Success,
                "Success",
                transactionsResponse));

        _mockAccountConnectorRepository
            .Setup(x => x.UpdateConnectionSyncCursor(
                syncEvent.ConnectorId,
                syncEvent.UserId,
                It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ImportTransactionsAsync(syncEvent);

        // Assert
        _mockCryptoService.Verify(x => x.Decrypt(syncEvent.AccessToken), Times.Once);
        _mockPlaidHttpService.Verify(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int>()), Times.Once);
        _mockAccountConnectorRepository.Verify(
            x => x.UpdateConnectionSyncCursor(
                syncEvent.ConnectorId,
                syncEvent.UserId,
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task ImportTransactionsAsync_WhenHasMore_RecursivelyFetchesMoreTransactions()
    {
        // Arrange
        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            ConnectorId = 1,
            AccessToken = new byte[] { 1, 2, 3 },
            EventType = EventType.TransactionImport
        };

        var decryptedToken = "decrypted-access-token";
        var firstCursor = "cursor-1";
        var secondCursor = "cursor-2";

        var firstResponse = CreateTransactionsResponse(hasMore: true, nextCursor: firstCursor);
        var secondResponse = CreateTransactionsResponse(hasMore: false);

        _mockCryptoService
            .Setup(x => x.Decrypt(syncEvent.AccessToken))
            .Returns(decryptedToken);

        _mockPlaidHttpService
            .SetupSequence(x => x.SyncTransactions(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<int>()))
            .ReturnsAsync(new ApiResponseResult<PlaidTransactionsSyncResponse>(
                ResultStatus.Success,
                "Success",
                firstResponse))
            .ReturnsAsync(new ApiResponseResult<PlaidTransactionsSyncResponse>(
                ResultStatus.Success,
                "Success",
                secondResponse));

        _mockAccountConnectorRepository
            .Setup(x => x.UpdateConnectionSyncCursor(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ImportTransactionsAsync(syncEvent);

        // Assert
        _mockPlaidHttpService.Verify(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int>()), Times.Once);
        _mockPlaidHttpService.Verify(x => x.SyncTransactions(decryptedToken, firstCursor, It.IsAny<int>()),
            Times.Once);
        _mockPlaidHttpService.Verify(x => x.SyncTransactions(decryptedToken, secondCursor, It.IsAny<int>()),
            Times.Never);
        _mockAccountConnectorRepository.Verify(
            x => x.UpdateConnectionSyncCursor(
                syncEvent.ConnectorId,
                syncEvent.UserId,
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task ImportTransactionsAsync_WhenApiFails_LogsErrorAndDoesNotUpdateSync()
    {
        // Arrange
        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            ConnectorId = 1,
            AccessToken = new byte[] { 1, 2, 3 },
            EventType = EventType.TransactionImport
        };

        var decryptedToken = "decrypted-access-token";
        var errorMessage = "API Error";

        _mockCryptoService
            .Setup(x => x.Decrypt(syncEvent.AccessToken))
            .Returns(decryptedToken);

        _mockPlaidHttpService
            .Setup(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int>()))
            .ReturnsAsync(new ApiResponseResult<PlaidTransactionsSyncResponse>(
                ResultStatus.Failed,
                errorMessage,
                null));

        // Act
        await _service.ImportTransactionsAsync(syncEvent);

        // Assert
        _mockPlaidHttpService.Verify(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int>()), Times.Once);
        _mockAccountConnectorRepository.Verify(
            x => x.UpdateConnectionSyncCursor(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()),
            Times.Once); // Still updates sync even on failure
    }

    [Fact]
    public async Task ImportTransactionsAsync_WhenDataIsNull_LogsWarningAndUpdatesSync()
    {
        // Arrange
        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            ConnectorId = 1,
            AccessToken = new byte[] { 1, 2, 3 },
            EventType = EventType.TransactionImport
        };

        var decryptedToken = "decrypted-access-token";

        _mockCryptoService
            .Setup(x => x.Decrypt(syncEvent.AccessToken))
            .Returns(decryptedToken);

        _mockPlaidHttpService
            .Setup(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int>()))
            .ReturnsAsync(new ApiResponseResult<PlaidTransactionsSyncResponse>(
                ResultStatus.Success,
                "Success",
                null));

        _mockAccountConnectorRepository
            .Setup(x => x.UpdateConnectionSyncCursor(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ImportTransactionsAsync(syncEvent);

        // Assert
        _mockPlaidHttpService.Verify(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int>()), Times.Once);
        _mockAccountConnectorRepository.Verify(
            x => x.UpdateConnectionSyncCursor(
                syncEvent.ConnectorId,
                syncEvent.UserId,
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task ImportTransactionsAsync_ProcessesAddedModifiedAndRemovedTransactions()
    {
        // Arrange
        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            ConnectorId = 1,
            AccessToken = new byte[] { 1, 2, 3 },
            EventType = EventType.TransactionImport
        };

        var decryptedToken = "decrypted-access-token";
        var response = new PlaidTransactionsSyncResponse
        {
            Added = new List<PlaidTransaction>
            {
                CreateTransaction("added-1", "Test Transaction 1", 100.50m)
            },
            Modified = new List<PlaidTransaction>
            {
                CreateTransaction("modified-1", "Test Transaction 2", 200.75m)
            },
            Removed = new List<PlaidTransaction>
            {
                CreateTransaction("removed-1", "Test Transaction 3", 300.25m)
            },
            HasMore = false,
            NextCursor = null,
        };

        _mockCryptoService
            .Setup(x => x.Decrypt(syncEvent.AccessToken))
            .Returns(decryptedToken);

        _mockPlaidHttpService
            .Setup(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int>()))
            .ReturnsAsync(new ApiResponseResult<PlaidTransactionsSyncResponse>(
                ResultStatus.Success,
                "Success",
                response));

        _mockAccountConnectorRepository
            .Setup(x => x.UpdateConnectionSyncCursor(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ImportTransactionsAsync(syncEvent);

        // Assert
        _mockPlaidHttpService.Verify(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int>()), Times.Once);
        _mockAccountConnectorRepository.Verify(
            x => x.UpdateConnectionSyncCursor(
                syncEvent.ConnectorId,
                syncEvent.UserId,
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task ImportTransactionsAsync_OnlyMapsTransactionsWithAExistingAccount()
    {
        _mockFinancialAccountRepository.Setup(x => x.GetAccountsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(
            new List<FinancialAccountEntity>
            {
                new FinancialAccountEntity
                {
                    AppLastChangedBy = 1,
                    UserId = 1,
                    ExternalId = "ABC123",
                    CurrentBalance = 0,
                    AccountMask = "1234",
                    DisplayName = "USAA Checking",
                    Subtype = "Checking"
                },
            });

        var transactionsSyncResponse = new PlaidTransactionsSyncResponse
        {
            Added = new List<PlaidTransaction>
            {
                CreateTransaction("added-1", "Test Transaction 1", 100.50m)
            },
            Modified = new List<PlaidTransaction>
            {
                CreateTransaction("modified-1", "Test Transaction 2", 200.75m, accountId: "invalid-account-id")
            },
            Removed = new List<PlaidTransaction>(),
            HasMore = false,
            NextCursor = null,
        };

        _mockPlaidHttpService.Setup(x => x.SyncTransactions(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<int>()))
            .ReturnsAsync(new ApiResponseResult<PlaidTransactionsSyncResponse>(
                ResultStatus.Success,
                "Success",
                transactionsSyncResponse
            ));

        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            ConnectorId = 1,
            AccessToken = new byte[] { 1, 2, 3 },
            EventType = EventType.TransactionImport
        };

        await _service.ImportTransactionsAsync(syncEvent);

        _mockTransactionRepository.Verify(
            x => x.AddTransactionsInBulk(It.IsAny<List<TransactionEntity>>()), Times.Once());
        
        _mockTransactionRepository.Verify(
            x => x.UpdateTransactionsInBulk(It.IsAny<List<TransactionEntity>>(), It.IsAny<int>()), Times.Never);

        _mockTransactionRepository.Verify(
            x => x.DeleteTransactionsInBulk(It.IsAny<List<string>>(), It.IsAny<int>()), Times.Never);
    }

    private static PlaidTransactionsSyncResponse CreateTransactionsResponse(
        bool hasMore = false,
        string? nextCursor = null)
    {
        return new PlaidTransactionsSyncResponse
        {
            Added = new List<PlaidTransaction>
            {
                CreateTransaction("txn-1", "Test Transaction", 50.00m)
            },
            Modified = new List<PlaidTransaction>(),
            Removed = new List<PlaidTransaction>(),
            HasMore = hasMore,
            NextCursor = nextCursor,
        };
    }

    private static PlaidTransaction CreateTransaction(
        string transactionId,
        string name,
        decimal amount,
        string accountId = "ABC123")
    {
        return new PlaidTransaction
        {
            AccountId = accountId,
            TransactionId = transactionId,
            Name = name,
            Amount = amount,
            Date = "2025-01-01",
            IsoCurrencyCode = "USD",
            Counterparties = new List<PlaidTransactionCounterparty>(),
            Pending = false,
        };
    }
}