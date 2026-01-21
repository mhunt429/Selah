using Domain.Events;
using Domain.Models.Entities.AccountConnector;
using Domain.Models.Entities.Mailbox;
using Domain.Models.Plaid;
using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Connector;
using Infrastructure.Services.Interfaces;
using Moq;

namespace Infrastructure.UnitTests.Services;

public class ConnectorEventServiceTests
{
    private readonly Mock<IPlaidAccountBalanceImportService> _plaidAccountBalanceImportService;
    private readonly Mock<IPlaidTransactionImportService> _plaidTransactionImportService;
    private readonly Mock<IUserMailboxRepository> _mailboxRepository;
    private readonly Mock<IAccountConnectorRepository> _accountConnectorRepository;

    private readonly ConnectorEventService _connectorEventService;

    public ConnectorEventServiceTests()
    {
        _plaidAccountBalanceImportService = new Mock<IPlaidAccountBalanceImportService>();
        _plaidTransactionImportService = new Mock<IPlaidTransactionImportService>();
        _mailboxRepository = new Mock<IUserMailboxRepository>();
        _accountConnectorRepository = new Mock<IAccountConnectorRepository>();

        _accountConnectorRepository.Setup(x => x.GetConnectorSyncRecordByConnectorId(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new AccountConnectorEntity
            {
                InstitutionId = "123",
                InstitutionName = "Wells Fargo",
                DateConnected = default,
                EncryptedAccessToken = new byte[]
                {
                },
                TransactionSyncCursor = null
            });

        _connectorEventService = new ConnectorEventService(_plaidAccountBalanceImportService.Object,
            _mailboxRepository.Object, _accountConnectorRepository.Object, _plaidTransactionImportService.Object);
    }

    [Fact]
    public async Task Service_Handles_HandleReauthNotification_WhenRequired()
    {
        var @event = new ConnectorDataSyncEvent
        {
            DataSyncId = 1,
            ConnectorId = 1,
            EventType = EventType.BalanceImport,
            Error = new PlaidApiErrorResponse
            {
                ErrorCode = ErrorCodes.LoginRequired
            }
        };

        await _connectorEventService.ProcessEventAsync(@event);

        _accountConnectorRepository.Verify(x => x.GetConnectorSyncRecordByConnectorId(It.IsAny<int>(), It.IsAny<int>()),
            Times.Once);

        _accountConnectorRepository.Verify(
            x => x.LockRecordWhenAuthenticationIsRequired(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

        _mailboxRepository.Verify(x => x.InsertMessage(It.IsAny<UserMailboxEntity>()), Times.Once);
    }
    
    [Fact]
    public async Task Service_ImportsAccountBalances_WhenRequired()
    {
        var @event = new ConnectorDataSyncEvent
        {
            DataSyncId = 1,
            ConnectorId = 1,
            EventType = EventType.BalanceImport,
            Error = null
        };
        
        await _connectorEventService.ProcessEventAsync(@event);
        
        _plaidAccountBalanceImportService.Verify(x => x.ImportAccountBalancesAsync(@event),Times.Once);
    }

    [Fact]
    public async Task Service_ImportsTransactions_WhenRequired()
    {
        var @event = new ConnectorDataSyncEvent
        {
            DataSyncId = 1,
            ConnectorId = 1,
            EventType = EventType.TransactionImport,
            Error = null
        };
        
        await _connectorEventService.ProcessEventAsync(@event);
        _plaidTransactionImportService.Verify(x => x.ImportTransactionsAsync(@event),Times.Once);
    }
}