using Domain.Events;
using Domain.Models;
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
    private readonly Mock<ILogger<PlaidTransactionImportService>> _mockLogger;
    private readonly Mock<IAccountConnectorRepository> _mockAccountConnectorRepository;
    private readonly Mock<TransactionRepository> _mockTransactionRepository;
    private readonly PlaidTransactionImportService _service;

    public PlaidTransactionImportServiceTests()
    {
        _mockCryptoService = new Mock<ICryptoService>();
        _mockPlaidHttpService = new Mock<IPlaidHttpService>();
        _mockLogger = new Mock<ILogger<PlaidTransactionImportService>>();
        _mockAccountConnectorRepository = new Mock<IAccountConnectorRepository>();
        _mockTransactionRepository = new Mock<TransactionRepository>();

        _service = new PlaidTransactionImportService(
            _mockCryptoService.Object,
            _mockPlaidHttpService.Object,
            _mockLogger.Object,
            _mockAccountConnectorRepository.Object,
            _mockTransactionRepository.Object);
    }

    [Fact]
    public async Task ImportTransactionsAsync_WhenSuccessful_ProcessesTransactionsAndUpdatesSync()
    {
        // Arrange
        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            DataSyncId = 1,
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
            .Setup(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int?>()))
            .ReturnsAsync(new ApiResponseResult<PlaidTransactionsSyncResponse>(
                ResultStatus.Success,
                "Success",
                transactionsResponse));

        _mockAccountConnectorRepository
            .Setup(x => x.UpdateConnectionSync(
                syncEvent.DataSyncId,
                syncEvent.UserId,
                It.IsAny<DateTimeOffset>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ImportTransactionsAsync(syncEvent);

        // Assert
        _mockCryptoService.Verify(x => x.Decrypt(syncEvent.AccessToken), Times.Once);
        _mockPlaidHttpService.Verify(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int?>()), Times.Once);
        _mockAccountConnectorRepository.Verify(
            x => x.UpdateConnectionSync(
                syncEvent.DataSyncId,
                syncEvent.UserId,
                It.IsAny<DateTimeOffset>()),
            Times.Once);
    }

    [Fact]
    public async Task ImportTransactionsAsync_WhenHasMore_RecursivelyFetchesMoreTransactions()
    {
        // Arrange
        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            DataSyncId = 1,
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
            .SetupSequence(x => x.SyncTransactions(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<int?>()))
            .ReturnsAsync(new ApiResponseResult<PlaidTransactionsSyncResponse>(
                ResultStatus.Success,
                "Success",
                firstResponse))
            .ReturnsAsync(new ApiResponseResult<PlaidTransactionsSyncResponse>(
                ResultStatus.Success,
                "Success",
                secondResponse));

        _mockAccountConnectorRepository
            .Setup(x => x.UpdateConnectionSync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTimeOffset>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ImportTransactionsAsync(syncEvent);

        // Assert
        _mockPlaidHttpService.Verify(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int?>()), Times.Once);
        _mockPlaidHttpService.Verify(x => x.SyncTransactions(decryptedToken, firstCursor, It.IsAny<int?>()),
            Times.Once);
        _mockPlaidHttpService.Verify(x => x.SyncTransactions(decryptedToken, secondCursor, It.IsAny<int?>()),
            Times.Never);
        _mockAccountConnectorRepository.Verify(
            x => x.UpdateConnectionSync(
                syncEvent.DataSyncId,
                syncEvent.UserId,
                It.IsAny<DateTimeOffset>()),
            Times.Once);
    }

    [Fact]
    public async Task ImportTransactionsAsync_WhenApiFails_LogsErrorAndDoesNotUpdateSync()
    {
        // Arrange
        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            DataSyncId = 1,
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
            .Setup(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int?>()))
            .ReturnsAsync(new ApiResponseResult<PlaidTransactionsSyncResponse>(
                ResultStatus.Failed,
                errorMessage,
                null));

        // Act
        await _service.ImportTransactionsAsync(syncEvent);

        // Assert
        _mockPlaidHttpService.Verify(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int?>()), Times.Once);
        _mockAccountConnectorRepository.Verify(
            x => x.UpdateConnectionSync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTimeOffset>()),
            Times.Once); // Still updates sync even on failure
    }

    [Fact]
    public async Task ImportTransactionsAsync_WhenDataIsNull_LogsWarningAndUpdatesSync()
    {
        // Arrange
        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            DataSyncId = 1,
            ConnectorId = 1,
            AccessToken = new byte[] { 1, 2, 3 },
            EventType = EventType.TransactionImport
        };

        var decryptedToken = "decrypted-access-token";

        _mockCryptoService
            .Setup(x => x.Decrypt(syncEvent.AccessToken))
            .Returns(decryptedToken);

        _mockPlaidHttpService
            .Setup(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int?>()))
            .ReturnsAsync(new ApiResponseResult<PlaidTransactionsSyncResponse>(
                ResultStatus.Success,
                "Success",
                null));

        _mockAccountConnectorRepository
            .Setup(x => x.UpdateConnectionSync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTimeOffset>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ImportTransactionsAsync(syncEvent);

        // Assert
        _mockPlaidHttpService.Verify(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int?>()), Times.Once);
        _mockAccountConnectorRepository.Verify(
            x => x.UpdateConnectionSync(
                syncEvent.DataSyncId,
                syncEvent.UserId,
                It.IsAny<DateTimeOffset>()),
            Times.Once);
    }

    [Fact]
    public async Task ImportTransactionsAsync_WhenHasMoreButCursorIsEmpty_DoesNotRecurse()
    {
        // Arrange
        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            DataSyncId = 1,
            ConnectorId = 1,
            AccessToken = new byte[] { 1, 2, 3 },
            EventType = EventType.TransactionImport
        };

        var decryptedToken = "decrypted-access-token";
        var response = CreateTransactionsResponse(hasMore: true, nextCursor: string.Empty);

        _mockCryptoService
            .Setup(x => x.Decrypt(syncEvent.AccessToken))
            .Returns(decryptedToken);

        _mockPlaidHttpService
            .Setup(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int?>()))
            .ReturnsAsync(new ApiResponseResult<PlaidTransactionsSyncResponse>(
                ResultStatus.Success,
                "Success",
                response));

        _mockAccountConnectorRepository
            .Setup(x => x.UpdateConnectionSync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTimeOffset>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ImportTransactionsAsync(syncEvent);

        // Assert
        _mockPlaidHttpService.Verify(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int?>()), Times.Once);
        _mockPlaidHttpService.Verify(x => x.SyncTransactions(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<int?>()),
            Times.Once);
    }

    [Fact]
    public async Task ImportTransactionsAsync_ProcessesAddedModifiedAndRemovedTransactions()
    {
        // Arrange
        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            DataSyncId = 1,
            ConnectorId = 1,
            AccessToken = new byte[] { 1, 2, 3 },
            EventType = EventType.TransactionImport
        };

        var decryptedToken = "decrypted-access-token";
        var response = new PlaidTransactionsSyncResponse
        {
            Accounts = new List<PlaidTransactionAccount>(),
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
            RequestId = "req-123",
            TransactionsUpdateStatus = "COMPLETE"
        };

        _mockCryptoService
            .Setup(x => x.Decrypt(syncEvent.AccessToken))
            .Returns(decryptedToken);

        _mockPlaidHttpService
            .Setup(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int?>()))
            .ReturnsAsync(new ApiResponseResult<PlaidTransactionsSyncResponse>(
                ResultStatus.Success,
                "Success",
                response));

        _mockAccountConnectorRepository
            .Setup(x => x.UpdateConnectionSync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTimeOffset>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ImportTransactionsAsync(syncEvent);

        // Assert
        _mockPlaidHttpService.Verify(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int?>()), Times.Once);
        _mockAccountConnectorRepository.Verify(
            x => x.UpdateConnectionSync(
                syncEvent.DataSyncId,
                syncEvent.UserId,
                It.IsAny<DateTimeOffset>()),
            Times.Once);
    }

    [Fact]
    public async Task ImportTransactionsAsync_WhenHasMoreAndNullCursor_DoesNotRecurse()
    {
        // Arrange
        var syncEvent = new ConnectorDataSyncEvent
        {
            UserId = 123,
            DataSyncId = 1,
            ConnectorId = 1,
            AccessToken = new byte[] { 1, 2, 3 },
            EventType = EventType.TransactionImport
        };

        var decryptedToken = "decrypted-access-token";
        var response = CreateTransactionsResponse(hasMore: true, nextCursor: null);

        _mockCryptoService
            .Setup(x => x.Decrypt(syncEvent.AccessToken))
            .Returns(decryptedToken);

        _mockPlaidHttpService
            .Setup(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int?>()))
            .ReturnsAsync(new ApiResponseResult<PlaidTransactionsSyncResponse>(
                ResultStatus.Success,
                "Success",
                response));

        _mockAccountConnectorRepository
            .Setup(x => x.UpdateConnectionSync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTimeOffset>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ImportTransactionsAsync(syncEvent);

        // Assert
        _mockPlaidHttpService.Verify(x => x.SyncTransactions(decryptedToken, null, It.IsAny<int?>()), Times.Once);
        _mockPlaidHttpService.Verify(x => x.SyncTransactions(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<int?>()),
            Times.Once);
    }

    private static PlaidTransactionsSyncResponse CreateTransactionsResponse(
        bool hasMore = false,
        string? nextCursor = null)
    {
        return new PlaidTransactionsSyncResponse
        {
            Accounts = new List<PlaidTransactionAccount>(),
            Added = new List<PlaidTransaction>
            {
                CreateTransaction("txn-1", "Test Transaction", 50.00m)
            },
            Modified = new List<PlaidTransaction>(),
            Removed = new List<PlaidTransaction>(),
            HasMore = hasMore,
            NextCursor = nextCursor,
            RequestId = "req-123",
            TransactionsUpdateStatus = "COMPLETE"
        };
    }

    private static PlaidTransaction CreateTransaction(
        string transactionId,
        string name,
        decimal amount)
    {
        return new PlaidTransaction
        {
            AccountId = "account-1",
            TransactionId = transactionId,
            Name = name,
            Amount = amount,
            Date = "2025-01-01",
            IsoCurrencyCode = "USD",
            Counterparties = new List<PlaidTransactionCounterparty>(),
            Pending = false
        };
    }
}