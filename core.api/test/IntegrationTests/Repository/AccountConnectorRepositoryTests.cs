using AwesomeAssertions;
using Domain.Models.Entities.AccountConnector;
using Infrastructure;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;
using IntegrationTests.Helpers;

namespace IntegrationTests.Repository;

[Collection("Database")]
public class AccountConnectorRepositoryTests(DatabaseFixture fixture) : IAsyncLifetime
{
    private readonly IAccountConnectorRepository _accountConnectorRepository = new AccountConnectorRepository(TestHelpers.TestDbFactory);
    private readonly BaseRepository _baseRepository = new(TestHelpers.TestDbFactory);
    private readonly AppDbContext _dbContext = TestHelpers.BuildTestDbContext();

    private int _userId;

    public async Task InitializeAsync()
    {
        await fixture.ResetDatabaseAsync();

        RegistrationRepository registrationRepository = new(_dbContext);
        var result = await TestHelpers.SetUpBaseRecords(registrationRepository);

        _userId = result.Item2.Id;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
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
            EncryptedAccessToken = "abc123"u8.ToArray(),
            TransactionSyncCursor = "",
        };
        int connectorId = await _accountConnectorRepository.InsertAccountConnectorRecord(data);

        var queryResult =
            await _baseRepository.GetFirstOrDefaultAsync<AccountConnectorEntity>(
                "SELECT * FROM account_connector WHERE user_id = @user_id", new { user_id = _userId });

        connectorId.Should().BeGreaterThan(0);
        queryResult.Should().NotBeNull();
        queryResult.UserId.Should().Be(_userId);
        queryResult.EncryptedAccessToken.Should().BeEquivalentTo(data.EncryptedAccessToken);
        queryResult.DateConnected.Should().BeAfter(DateTimeOffset.MinValue);
        queryResult.InstitutionId.Should().Be(data.InstitutionId);
        queryResult.InstitutionName.Should().Be(data.InstitutionName);

        var connectionSync =
            await _accountConnectorRepository.GetConnectorSyncRecordByConnectorId(_userId, connectorId);
        connectionSync.Should().NotBeNull();
        connectionSync.UserId.Should().Be(_userId);
        connectionSync.Id.Should().BeGreaterThan(0);
        connectionSync.ConnectorId.Should().Be(connectorId);
        connectionSync.LastSyncDate.Should().BeAfter(DateTimeOffset.MinValue);
        connectionSync.NextSyncDate.Should().BeAfter(DateTimeOffset.MinValue);
        connectionSync.EncryptedAccessToken.Should().BeEquivalentTo(data.EncryptedAccessToken);
    }
}