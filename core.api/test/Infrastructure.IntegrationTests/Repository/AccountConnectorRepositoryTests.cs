using Domain.Models.Entities.AccountConnector;
using FluentAssertions;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Respawn;
using Respawn.Graph;

namespace Infrastructure.IntegrationTests.Repository;

public class AccountConnectorRepositoryTests : IAsyncLifetime
{
    private readonly IAccountConnectorRepository _accountConnectorRepository;
    private readonly BaseRepository _baseRepository = new(TestHelpers.TestDbFactory);
    private readonly AppDbContext _dbContext = TestHelpers.BuildTestDbContext();

    private int _accountId;
    private Respawner _respawner;

    private int _userId;

    public AccountConnectorRepositoryTests()
    {
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
        _accountId = result.Item1.Id;
        _userId = result.Item2.Id;
        ;
    }

    public async Task DisposeAsync()
    {
        using var conn = _dbContext.Database.GetDbConnection();
        await conn.OpenAsync();
        await _respawner.ResetAsync(conn);
    }

    [Fact]
    public async Task InsertAccountConnectorRecord_ShouldSaveRecord()
    {
        AccountConnectorEntity data = new()
        {
            UserId = _userId,
            InstitutionId = "123",
            InstitutionName = "Morgan Stanley",
            DateConnected = DateTimeOffset.UtcNow,
            EncryptedAccessToken = "token",
            TransactionSyncCursor = "",
            OriginalInsert = DateTimeOffset.UtcNow
        };
        await _accountConnectorRepository.InsertAccountConnectorRecord(data);

        var queryResult =
            await _baseRepository.GetFirstOrDefaultAsync<AccountConnectorEntity>(
                "SELECT * FROM account_connector WHERE user_id = @user_id", new { user_id = _userId });

        queryResult.Should().NotBeNull();
        queryResult.UserId.Should().Be(_userId);
        queryResult.EncryptedAccessToken.Should().Be(data.EncryptedAccessToken);
        queryResult.DateConnected.Should().BeAfter(DateTimeOffset.MinValue);
        queryResult.InstitutionId.Should().Be(data.InstitutionId);
        queryResult.InstitutionName.Should().Be(data.InstitutionName);
    }
}