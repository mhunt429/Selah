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

    private readonly IAccountConnectorRepository _accountConnectorRepository =
        new AccountConnectorRepository(_dbContext);

    private int _userId;
    private int _connectorId;

    private AccountConnectorEntity? _sampleData;

    private AccountConnectorEntity? _disconnectedRecord;

    public async Task InitializeAsync()
    {
        await fixture.ResetDatabaseAsync();
        var result = await TestHelpers.SetUpBaseRecords(_dbContext);

        _userId = result.Item2.Id;

        _sampleData = new()
        {
            AppLastChangedBy = _userId,
            UserId = _userId,
            InstitutionId = "123",
            InstitutionName = "Morgan Stanley",
            DateConnected = DateTimeOffset.UtcNow,
            EncryptedAccessToken = "abc123"u8.ToArray(),
            TransactionSyncCursor = "",
            RequiresReauthentication = false,
            LastSyncDate = DateTimeOffset.UtcNow,
            NextSyncDate = DateTimeOffset.UtcNow.AddDays(3)
        };
        _connectorId = await _accountConnectorRepository.InsertAccountConnectorRecord(_sampleData);
        _connectorId.Should().BeGreaterThan(0);

        _disconnectedRecord = _sampleData;
        _disconnectedRecord.Id = 0;
        _disconnectedRecord.RequiresReauthentication = true;
        _disconnectedRecord.DisconnectedTs = DateTimeOffset.UtcNow;
        await _accountConnectorRepository.InsertAccountConnectorRecord(_disconnectedRecord);
        _disconnectedRecord.Id.Should().BeGreaterThan(0);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetConnectorRecordByIdAndUser_ShouldReturnRecord()
    {
        var connectionSync =
            await _accountConnectorRepository.GetConnectorRecordByIdAndUser(_userId, _connectorId);
        connectionSync.Should().NotBeNull();
        connectionSync.UserId.Should().Be(_userId);
        connectionSync.Id.Should().BeGreaterThan(0);
        connectionSync.Id.Should().Be(_connectorId);
        connectionSync.LastSyncDate.Should().BeAfter(DateTimeOffset.MinValue);
        connectionSync.NextSyncDate.Should().BeAfter(DateTimeOffset.MinValue);
        connectionSync.EncryptedAccessToken.Should().BeEquivalentTo(_sampleData!.EncryptedAccessToken);
        connectionSync.RequiresReauthentication.Should().BeFalse();
        connectionSync.InstitutionName.Should().Be(_sampleData.InstitutionName);
    }

    [Fact]
    public async Task RemoveConnectionSyncLock_ShouldClearLock()
    {
        var cleared =
            await _accountConnectorRepository.RemoveConnectionSyncLock(_disconnectedRecord!.Id,
                _disconnectedRecord.UserId);
        cleared.Should().BeTrue();

        var queryResult =
            await _accountConnectorRepository.GetConnectorRecordByIdAndUser(_disconnectedRecord.UserId,
                _disconnectedRecord!.Id);
        queryResult.Should().NotBeNull();
        queryResult.DisconnectedTs.Should().BeNull();
        queryResult.RequiresReauthentication.Should().BeFalse();
    }
}