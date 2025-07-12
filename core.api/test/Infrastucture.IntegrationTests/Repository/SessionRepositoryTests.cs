using FluentAssertions;
using Domain.Models.Entities.Identity;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;

namespace Infrastructure.IntegrationTests.Repository;

public class SessionRepositoryTests : IAsyncLifetime
{
    private readonly BaseRepository _baseRepository = new BaseRepository(TestHelpers.TestDbFactory);
    private readonly AppDbContext _dbContext = TestHelpers.BuildTestDbContext();

    private Guid _accountId = Guid.NewGuid();
    private Guid _userId = Guid.NewGuid();


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
            SessionId = Guid.NewGuid(),
        };

        await _userSessionRepository.IssueSession(session);

        session = await _userSessionRepository.GetUserSessionAsync(_userId);
        session.Should().NotBeNull();
        session.SessionId.Should().Be(session.SessionId);
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
        await TestHelpers.SetUpBaseRecords(_accountId, _userId, registrationRepository);
    }

    public async Task DisposeAsync()
    {
        await TestHelpers.TearDownBaseRecords(_userId, _accountId, _baseRepository);

        string accountConnectorDelete = "DELETE FROM account_connector WHERE user_id = @user_id";
        await _baseRepository.DeleteAsync(accountConnectorDelete, new { user_id = _userId });
    }
}