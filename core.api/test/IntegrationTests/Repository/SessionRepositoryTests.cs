using AwesomeAssertions;
using Domain.Models.Entities.Identity;
using Infrastructure;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;
using IntegrationTests.Helpers;

namespace IntegrationTests.Repository;

[Collection("Database")]
public class SessionRepositoryTests : IAsyncLifetime
{
    private readonly AppDbContext _dbContext = TestHelpers.BuildTestDbContext();
    private readonly DatabaseFixture _fixture;
    private readonly IUserSessionRepository _userSessionRepository;

    private int _userId;

    public SessionRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _userSessionRepository = new UserSessionRepository(_dbContext);
    }


    public async Task InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();
        var result = await TestHelpers.SetUpBaseRecords(_dbContext);
        _userId = result.Item2.Id;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Repository_ShouldBeAbleToIssueAndRevokeSessions()
    {
        var session = new UserSessionEntity
        {
            AppLastChangedBy = _userId,
            Id = Guid.NewGuid(),
            UserId = _userId,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(1),
            IssuedAt = DateTimeOffset.UtcNow,
        };

        await _userSessionRepository.IssueSession(session);

        session = await _userSessionRepository.GetUserSessionAsync(_userId);
        session.Should().NotBeNull();
        session.ExpiresAt.Should().Be(session.ExpiresAt);
        session.AppLastChangedBy.Should().Be(session.AppLastChangedBy);
        session.UserId.Should().Be(session.UserId);

        await _userSessionRepository.RevokeSessionsByUser(_userId);
        session = await _userSessionRepository.GetUserSessionAsync(_userId);
        session.Should().BeNull();
    }
}