using Domain.Models.Entities.AccountConnector;
using Domain.Models.Entities.FinancialAccount;
using FluentAssertions;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Respawn;
using Respawn.Graph;

namespace Infrastructure.IntegrationTests.Repository;

public class FinancialAccountRepositoryTests : IAsyncLifetime
{
    private readonly IAccountConnectorRepository _accountConnectorRepository;
    private readonly BaseRepository _baseRepository = new(TestHelpers.TestDbFactory);
    private readonly IDbConnectionFactory _dbConnectionFactory = TestHelpers.TestDbFactory;

    private readonly AppDbContext _dbContext = TestHelpers.BuildTestDbContext();

    private readonly IFinancialAccountRepository _financialAccountRepository;


    private int _connectorId;
    private Respawner _respawner;
    private int _userId;


    public FinancialAccountRepositoryTests()
    {
        _financialAccountRepository = new FinancialAccountRepository(_dbContext, _dbConnectionFactory);
        _accountConnectorRepository = new AccountConnectorRepository(_dbContext);
    }

    public async Task InitializeAsync()
    {
        await using var conn = new NpgsqlConnection(TestHelpers.TestConnectionString);
        await conn.OpenAsync();

        _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = new[] { new Table("flyway_schema_history") } // ignore migration table
        });

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
            EncryptedAccessToken = "token",
            TransactionSyncCursor = "",
            OriginalInsert = DateTimeOffset.UtcNow
        };
        _connectorId = await _accountConnectorRepository.InsertAccountConnectorRecord(data);
    }

    public async Task DisposeAsync()
    {
        using var conn = _dbContext.Database.GetDbConnection();
        await conn.OpenAsync();
        await _respawner.ResetAsync(conn);
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
                OriginalInsert = DateTimeOffset.UtcNow,
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
                OriginalInsert = DateTimeOffset.UtcNow,
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
            OriginalInsert = DateTimeOffset.UtcNow,
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
            OriginalInsert = DateTimeOffset.UtcNow
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
            OriginalInsert = DateTimeOffset.UtcNow
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
            OriginalInsert = DateTimeOffset.UtcNow,
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

        // wait for the balance history to be inserted and new user record
        await Task.Delay(5000);
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