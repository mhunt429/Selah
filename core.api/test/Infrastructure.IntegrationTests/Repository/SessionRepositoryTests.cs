using Domain.Models.Entities.Identity;
using FluentAssertions;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Respawn;
using Respawn.Graph;

namespace Infrastructure.IntegrationTests.Repository;

public class SessionRepositoryTests : IAsyncLifetime
{
    private readonly AppDbContext _dbContext = TestHelpers.BuildTestDbContext();


    private readonly IUserSessionRepository _userSessionRepository;
    private Respawner _respawner;
    private int _userId;

    public SessionRepositoryTests()
    {
        _userSessionRepository = new UserSessionRepository(_dbContext);
    }


    public async Task InitializeAsync()
    {
        await using var conn = new NpgsqlConnection(TestHelpers.TestConnectionString);
        await conn.OpenAsync();

        _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres, // explicitly PostgreSQL
            TablesToIgnore = new[] { new Table("flyway_schema_history") } // ignore migration table
        });
        var registrationRepository = new RegistrationRepository(_dbContext);
        var result = await TestHelpers.SetUpBaseRecords(registrationRepository);
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
    public async Task Repository_ShouldBeAbleToIssueAndRevokeSessions()
    {
        var session = new UserSessionEntity
        {
            OriginalInsert = DateTimeOffset.UtcNow,
            AppLastChangedBy = _userId,
            Id = Guid.NewGuid(),
            UserId = _userId,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(1)
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
}