using Domain.Models.Entities.AccountConnector;
using Domain.Models.Entities.ApplicationUser;
using Domain.Models.Entities.FinancialAccount;
using Domain.Models.Entities.UserAccount;
using FluentAssertions;
using Infrastructure;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;

namespace Infrastructure.IntegrationTests.Repository;

public class FinancialAccountRepositoryTests : IAsyncLifetime
{
    private readonly BaseRepository _baseRepository = new BaseRepository(TestHelpers.TestDbFactory);

    private readonly AppDbContext _dbContext = TestHelpers.BuildTestDbContext();
    private readonly IDbConnectionFactory _dbConnectionFactory = TestHelpers.TestDbFactory;

    private readonly IFinancialAccountRepository _financialAccountRepository;
    private readonly IAccountConnectorRepository _accountConnectorRepository;

    private int _accountId;
    private int _userId;

    private int _connectorId;


    public FinancialAccountRepositoryTests()
    {
        _financialAccountRepository = new FinancialAccountRepository(_dbContext, _dbConnectionFactory);
        _accountConnectorRepository = new AccountConnectorRepository(_dbContext);
    }

    [Fact]
    public async Task ImportFinancialAccountsAsync_ShouldInsertMultipleAccounts()
    {
        var data = new List<FinancialAccountEntity>
        {
            new FinancialAccountEntity
            {
                AppLastChangedBy = _userId,
                UserId = _userId,
                ExternalId = "1234",
                AccountMask = "***111",
                CurrentBalance = 100,
                DisplayName = "My Checking",
                OfficialName = "RBC Personal Checking",
                Subtype = "Checking",
                IsExternalApiImport = true,
                LastApiSyncTime = DateTimeOffset.UtcNow,
                OriginalInsert = DateTimeOffset.UtcNow,
                ConnectorId = _connectorId,
            },
            new FinancialAccountEntity
            {
                AppLastChangedBy = _userId,
                UserId = _userId,
                ExternalId = "4321",
                AccountMask = "***4321",
                DisplayName = "My Saving",
                CurrentBalance = 500,
                OfficialName = "RBC Personal Savings",
                Subtype = "Savings",
                IsExternalApiImport = true,
                LastApiSyncTime = DateTimeOffset.UtcNow,
                OriginalInsert = DateTimeOffset.UtcNow,
                ConnectorId = _connectorId,
            },
        };

        await _financialAccountRepository.ImportFinancialAccountsAsync(data);

        var result = await _financialAccountRepository.GetAccountsAsync(_userId);

        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task AddAccountAsync_ShouldInsertNewAccount()
    {
        var account = new FinancialAccountEntity
        {
            AppLastChangedBy = _userId,
            UserId = _userId,
            ExternalId = "4321",
            AccountMask = "***4321",
            DisplayName = "Vanguard Trust 401k",
            CurrentBalance = 500,
            OfficialName = "Vanguard Total Trust 401k",
            Subtype = "Retirement",
            IsExternalApiImport = true,
            LastApiSyncTime = DateTimeOffset.UtcNow,
            OriginalInsert = DateTimeOffset.UtcNow,
            ConnectorId = _connectorId,
        };

        int newAccountId = await _financialAccountRepository.AddAccountAsync(account);

        var result = await _financialAccountRepository.GetAccountByIdAsync(_userId, newAccountId);
        result.Should().NotBeNull();
        result.Id.Should().Be(newAccountId);
        result.UserId.Should().Be(_userId);
        result.ExternalId.Should().Be("4321");
        //result.AccountMask.Should().Be("***4321");
        result.DisplayName.Should().Be("Vanguard Trust 401k");
        result.CurrentBalance.Should().Be(500);
        result.OfficialName.Should().Be("Vanguard Total Trust 401k");
        result.Subtype.Should().Be("Retirement");
        result.IsExternalApiImport.Should().BeTrue();
        result.LastApiSyncTime.Should().BeAfter(DateTimeOffset.MinValue);
    }

    [Fact]
    public async Task UpdateAccountAsync_ShouldUpdateAccount()
    {
        var account = new FinancialAccountEntity
        {
            AppLastChangedBy = _userId,
            UserId = _userId,
            ExternalId = "4321",
            AccountMask = "***4321",
            DisplayName = "Vanguard Trust 401k",
            CurrentBalance = 500,
            OfficialName = "Vanguard Total Trust 401k",
            Subtype = "Retirement",
            IsExternalApiImport = true,
            LastApiSyncTime = DateTimeOffset.UtcNow,
            ConnectorId = _connectorId,
            OriginalInsert = DateTimeOffset.UtcNow,
        };

        int newAccountId = await _financialAccountRepository.AddAccountAsync(account);

        var accountUpdate = new FinancialAccountUpdate()
        {
            CurrentBalance = 1000,
            DisplayName = "Vanguard Trust 401k",
            OfficialName = "Vanguard Total Trust 401k",
            Subtype = "Retirement",
            LastApiSyncTime = DateTimeOffset.UtcNow,
        };

        await _financialAccountRepository.UpdateAccount(accountUpdate, newAccountId, _userId);
        var result = await _financialAccountRepository.GetAccountByIdAsync(_userId, newAccountId);
        result.Should().NotBeNull();
        result.CurrentBalance.Should().Be(1000);
    }

    [Fact]
    public async Task DeleteAccountAsync_ShouldDeleteAccount()
    {
        var account = new FinancialAccountEntity
        {
            AppLastChangedBy = _userId,
            UserId = _userId,
            ExternalId = "4321",
            AccountMask = "***4321",
            DisplayName = "Vanguard Trust 401k",
            CurrentBalance = 500,
            OfficialName = "Vanguard Total Trust 401k",
            Subtype = "Retirement",
            IsExternalApiImport = true,
            LastApiSyncTime = DateTimeOffset.UtcNow,
            ConnectorId = _connectorId,
            OriginalInsert = DateTimeOffset.UtcNow,
        };

        int newAccountId = await _financialAccountRepository.AddAccountAsync(account);

        var deleteResult = await _financialAccountRepository.DeleteAccountAsync(account);
        deleteResult.Should().BeTrue();

        var result = await _financialAccountRepository.GetAccountByIdAsync(_userId, newAccountId);
        result.Should().BeNull();
    }

    [Fact]
    public async Task Account_Balance_History_Should_Be_Inserted()
    {
        var account = new FinancialAccountEntity
        {
            AppLastChangedBy = _userId,
            UserId = _userId,
            ExternalId = "4321",
            AccountMask = "***4321",
            DisplayName = "Vanguard Trust 401k",
            CurrentBalance = 500,
            OfficialName = "Vanguard Total Trust 401k",
            Subtype = "Retirement",
            IsExternalApiImport = true,
            LastApiSyncTime = DateTimeOffset.UtcNow,
            OriginalInsert = DateTimeOffset.UtcNow,
            ConnectorId = _connectorId,
        };

        int newAccountId = await _financialAccountRepository.AddAccountAsync(account);

        var accountBalanceHistory = new AccountBalanceHistoryEntity
        {
            UserId = _userId,
            FinancialAccountId = newAccountId,
            CurrentBalance = 250.00m,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        // wait for the balance history to be inserted and new user record
        await Task.Delay(5000);
        await _financialAccountRepository.InsertBalanceHistory(accountBalanceHistory, _userId);

        var result = await _financialAccountRepository.GetBalanceHistory(_userId, newAccountId);

        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result.First().FinancialAccountId.Should().Be(newAccountId);
        result.First().UserId.Should().Be(_userId);
        result.First().CurrentBalance.Should().Be(250.00m);
        result.First().CreatedAt.Should().Be(accountBalanceHistory.CreatedAt);
    }

    public async Task InitializeAsync()
    {
        var registrationRepository = new RegistrationRepository(_dbContext);
        (UserAccountEntity, ApplicationUserEntity) result = await TestHelpers.SetUpBaseRecords(registrationRepository);
        _accountId = result.Item1.Id;
        _userId = result.Item2.Id;
        ;

        AccountConnectorEntity data = new AccountConnectorEntity
        {
            AppLastChangedBy = _userId,
            UserId = _userId,
            InstitutionId = "123",
            InstitutionName = "Morgan Stanley",
            DateConnected = DateTimeOffset.UtcNow,
            EncryptedAccessToken = "token",
            TransactionSyncCursor = "",
            OriginalInsert = DateTimeOffset.UtcNow,
        };
        _connectorId = await _accountConnectorRepository.InsertAccountConnectorRecord(data);
    }

    public async Task DisposeAsync()
    {
        string balanceHistoryDelete = "DELETE FROM account_balance_history WHERE user_id = @user_id";
        await _baseRepository.DeleteAsync(balanceHistoryDelete, new { user_id = _userId });

        string financialAccountDelete = "DELETE FROM financial_account WHERE user_id = @user_id";
        await _baseRepository.DeleteAsync(financialAccountDelete, new { user_id = _userId });

        string accountConnectorDelete = "DELETE FROM account_connector WHERE user_id = @user_id";
        await _baseRepository.DeleteAsync(accountConnectorDelete, new { user_id = _userId });


        await TestHelpers.TearDownBaseRecords(_userId, _accountId, _baseRepository);
    }
}