using Domain.Models.Entities.ApplicationUser;
using FluentAssertions;
using Domain.Models.Entities.Identity;
using Domain.Models.Entities.UserAccount;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;

namespace Infrastructure.IntegrationTests.Repository;

public class SessionRepositoryTests : IAsyncLifetime
{
    private readonly BaseRepository _baseRepository = new BaseRepository(TestHelpers.TestDbFactory);
    private readonly AppDbContext _dbContext = TestHelpers.BuildTestDbContext();

    private int _accountId;
    private int _userId;


    private IUserSessionRepository _userSessionRepository;

    public SessionRepositoryTests()
    {
        _userSessionRepository = new UserSessionRepository(_dbContext);
    }

    [Fact]
    public async Task Repository_ShouldBeAbleToIssueAndRevokeSessions()
    {
        var session = new UserSessionEntity
        {
            OriginalInsert = DateTimeOffset.UtcNow,
            AppLastChangedBy = _userId,
            Id = Guid.NewGuid(),
            UserId = _userId,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(1),
        };

        await _userSessionRepository.IssueSession(session);

        session = await _userSessionRepository.GetUserSessionAsync(_userId);
        session.Should().NotBeNull();
        session.ExpiresAt.Should().Be(session.ExpiresAt);
        session.AppLastChangedBy.Should().Be(session.AppLastChangedBy);
        session.OriginalInsert.Should().Be(session.OriginalInsert);
        session.UserId.Should().Be(session.UserId);

        await _userSessionRepository.RevokeSessionsByUser(_userId, true);
        session = await _userSessionRepository.GetUserSessionAsync(_userId);
        session.Should().BeNull();
    }


    public async Task InitializeAsync()
    {
        var registrationRepository = new RegistrationRepository(_dbContext);
        (UserAccountEntity, ApplicationUserEntity) result = await TestHelpers.SetUpBaseRecords(registrationRepository);
        _accountId = result.Item1.Id;
        _userId = result.Item2.Id;;
    }

    public async Task DisposeAsync()
    {
        await TestHelpers.TearDownBaseRecords(_userId, _accountId, _baseRepository);

        string accountConnectorDelete = "DELETE FROM account_connector WHERE user_id = @user_id";
        await _baseRepository.DeleteAsync(accountConnectorDelete, new { user_id = _userId });
    }
}