using FluentAssertions;
using Domain.Models.Entities.AccountConnector;
using Domain.Models.Entities.ApplicationUser;
using Domain.Models.Entities.UserAccount;
using Infrastructure.Repository;

namespace Infrastructure.IntegrationTests.Repository;

public class AccountConnectorRepositoryTests : IAsyncLifetime
{
    private readonly BaseRepository _baseRepository = new BaseRepository(TestHelpers.TestDbFactory);

    private readonly AppDbContext _dbContext = TestHelpers.BuildTestDbContext();

    private readonly IAccountConnectorRepository _accountConnectorRepository;

    private int _accountId;
    private int _userId;

    public AccountConnectorRepositoryTests()
    {
        _accountConnectorRepository = new AccountConnectorRepository(_dbContext);
    }

    [Fact]
    public async Task InsertAccountConnectorRecord_ShouldSaveRecord()
    {
        AccountConnectorEntity data = new AccountConnectorEntity
        {
            UserId = _userId,
            InstitutionId = "123",
            InstitutionName = "Morgan Stanley",
            DateConnected = DateTimeOffset.UtcNow,
            EncryptedAccessToken = "token",
            TransactionSyncCursor = "",
            OriginalInsert = DateTimeOffset.UtcNow,
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

    public async Task InitializeAsync()
    {
        var registrationRepository = new RegistrationRepository(_dbContext);
        (UserAccountEntity, ApplicationUserEntity) result = await TestHelpers.SetUpBaseRecords(registrationRepository);
        _accountId = result.Item1.Id;
        _userId = result.Item2.Id;
        ;
    }

    public async Task DisposeAsync()
    {
        string accountConnectorDelete = "DELETE FROM account_connector WHERE user_id = @user_id";
        await _baseRepository.DeleteAsync(accountConnectorDelete, new { user_id = _userId });
        await TestHelpers.TearDownBaseRecords(_userId, _accountId, _baseRepository);
    }
}