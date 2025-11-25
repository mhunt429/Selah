using AwesomeAssertions;
using Domain.Models.Entities.AccountConnector;
using Domain.Models.Entities.FinancialAccount;
using Infrastructure;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;
using IntegrationTests.Helpers;

namespace IntegrationTests.Repository;

[Collection("Database")]
public class FinancialAccountRepositoryTests : IAsyncLifetime
{
    private readonly IAccountConnectorRepository _accountConnectorRepository;
    private readonly BaseRepository _baseRepository = new(TestHelpers.TestDbFactory);
    private readonly IDbConnectionFactory _dbConnectionFactory = TestHelpers.TestDbFactory;
    private readonly AppDbContext _dbContext = TestHelpers.BuildTestDbContext();

    private readonly IFinancialAccountRepository _financialAccountRepository;
    private readonly DatabaseFixture _fixture;


    private int _connectorId;
    private int _userId;


    public FinancialAccountRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _financialAccountRepository = new FinancialAccountRepository(_dbContext, _dbConnectionFactory);
        _accountConnectorRepository = new AccountConnectorRepository(TestHelpers.TestDbFactory);
    }

    public async Task InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();

        RegistrationRepository registrationRepository = new(_dbContext);
        var result = await TestHelpers.SetUpBaseRecords(registrationRepository);
        _userId = result.Item2.Id;

        AccountConnectorEntity data = new()
        {
            AppLastChangedBy = _userId,
            UserId = _userId,
            InstitutionId = "123",
            InstitutionName = "Morgan Stanley",
            DateConnected = DateTimeOffset.UtcNow,
            EncryptedAccessToken = "abc123"u8.ToArray(),
            TransactionSyncCursor = "",
        };
        _connectorId = await _accountConnectorRepository.InsertAccountConnectorRecord(data);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task ImportFinancialAccountsAsync_ShouldInsertMultipleAccounts()
    {
        List<FinancialAccountEntity> data = new()
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
                ConnectorId = _connectorId
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
                ConnectorId = _connectorId
            }
        };

        await _financialAccountRepository.ImportFinancialAccountsAsync(data);

        var result = await _financialAccountRepository.GetAccountsAsync(_userId);

        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task AddAccountAsync_ShouldInsertNewAccount()
    {
        FinancialAccountEntity account = new()
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
            ConnectorId = _connectorId
        };

        var newAccountId = await _financialAccountRepository.AddAccountAsync(account);

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
        FinancialAccountEntity account = new()
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
        };

        var newAccountId = await _financialAccountRepository.AddAccountAsync(account);

        FinancialAccountUpdate accountUpdate = new()
        {
            CurrentBalance = 1000,
            DisplayName = "Vanguard Trust 401k",
            OfficialName = "Vanguard Total Trust 401k",
            Subtype = "Retirement",
            LastApiSyncTime = DateTimeOffset.UtcNow
        };

        await _financialAccountRepository.UpdateAccount(accountUpdate, newAccountId, _userId);
        var result = await _financialAccountRepository.GetAccountByIdAsync(_userId, newAccountId);
        result.Should().NotBeNull();
        result.CurrentBalance.Should().Be(1000);
    }

    [Fact]
    public async Task DeleteAccountAsync_ShouldDeleteAccount()
    {
        FinancialAccountEntity account = new()
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
        };

        var newAccountId = await _financialAccountRepository.AddAccountAsync(account);

        var deleteResult = await _financialAccountRepository.DeleteAccountAsync(account);
        deleteResult.Should().BeTrue();

        var result = await _financialAccountRepository.GetAccountByIdAsync(_userId, newAccountId);
        result.Should().BeNull();
    }

    [Fact]
    public async Task Account_Balance_History_Should_Be_Inserted()
    {
        FinancialAccountEntity account = new()
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
            ConnectorId = _connectorId
        };

        var newAccountId = await _financialAccountRepository.AddAccountAsync(account);

        AccountBalanceHistoryEntity accountBalanceHistory = new()
        {
            UserId = _userId,
            FinancialAccountId = newAccountId,
            CurrentBalance = 250.00m,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _financialAccountRepository.InsertBalanceHistory(accountBalanceHistory, _userId);

        var result =
            await _financialAccountRepository.GetBalanceHistory(_userId, newAccountId);

        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result.First().FinancialAccountId.Should().Be(newAccountId);
        result.First().UserId.Should().Be(_userId);
        result.First().CurrentBalance.Should().Be(250.00m);
        result.First().CreatedAt.Should().BeAfter(DateTimeOffset.MinValue);
    }
}