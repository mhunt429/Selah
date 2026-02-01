using AwesomeAssertions;
using Domain.Models.Entities.AccountConnector;
using Infrastructure;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;
using IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Repository;

[Collection("Database")]
public class AccountConnectorRepositoryTests(DatabaseFixture fixture) : IAsyncLifetime
{

    private static readonly AppDbContext _dbContext = TestHelpers.BuildTestDbContext();
    
    private readonly IAccountConnectorRepository _accountConnectorRepository = new AccountConnectorRepository(_dbContext);

    private int _userId;

    public async Task InitializeAsync()
    {
        await fixture.ResetDatabaseAsync();
        var result = await TestHelpers.SetUpBaseRecords(_dbContext);

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
            AppLastChangedBy = _userId,
            UserId = _userId,
            InstitutionId = "123",
            InstitutionName = "Morgan Stanley",
            DateConnected = DateTimeOffset.UtcNow,
            EncryptedAccessToken = "abc123"u8.ToArray(),
            TransactionSyncCursor = "",
            RequiresReauthentication =  false,
            LastSyncDate = DateTimeOffset.UtcNow,
            NextSyncDate = DateTimeOffset.UtcNow.AddDays(3)
        };
        int connectorId = await _accountConnectorRepository.InsertAccountConnectorRecord(data);

        var queryResult = await _dbContext.AccountConnectors
            .FirstOrDefaultAsync(x => x.UserId == _userId);
        
        connectorId.Should().BeGreaterThan(0);
        queryResult.Should().NotBeNull();
        queryResult.UserId.Should().Be(_userId);
        queryResult.EncryptedAccessToken.Should().BeEquivalentTo(data.EncryptedAccessToken);
        queryResult.DateConnected.Should().BeAfter(DateTimeOffset.MinValue);
        queryResult.InstitutionId.Should().Be(data.InstitutionId);
        queryResult.InstitutionName.Should().Be(data.InstitutionName);

        var connectionSync =
            await _accountConnectorRepository.GetConnectorRecordByIdAndUser(_userId, connectorId);
        connectionSync.Should().NotBeNull();
        connectionSync.UserId.Should().Be(_userId);
        connectionSync.Id.Should().BeGreaterThan(0);
        connectionSync.Id.Should().Be(connectorId);
        connectionSync.LastSyncDate.Should().BeAfter(DateTimeOffset.MinValue);
        connectionSync.NextSyncDate.Should().BeAfter(DateTimeOffset.MinValue);
        connectionSync.EncryptedAccessToken.Should().BeEquivalentTo(data.EncryptedAccessToken);
    }
}