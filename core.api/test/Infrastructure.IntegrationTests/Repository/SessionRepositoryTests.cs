using Domain.Models.Entities.Identity;
using FluentAssertions;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;

namespace Infrastructure.IntegrationTests.Repository;

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
        _userSessionRepository = new UserSessionRepository(TestHelpers.TestDbFactory);
    }


    public async Task InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();
        var registrationRepository = new RegistrationRepository(_dbContext);
        var result = await TestHelpers.SetUpBaseRecords(registrationRepository);
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
            OriginalInsert = DateTimeOffset.UtcNow,
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
        session.OriginalInsert.Should().Be(session.OriginalInsert);
        session.UserId.Should().Be(session.UserId);

        await _userSessionRepository.RevokeSessionsByUser(_userId);
        session = await _userSessionRepository.GetUserSessionAsync(_userId);
        session.Should().BeNull();
    }
}