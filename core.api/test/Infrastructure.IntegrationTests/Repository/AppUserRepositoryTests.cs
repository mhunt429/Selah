using FluentAssertions;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Respawn;
using Respawn.Graph;

namespace Infrastructure.IntegrationTests.Repository;

public class AppUserRepositoryTests : IAsyncLifetime
{
    private readonly BaseRepository _baseRepository = new(TestHelpers.TestDbFactory);
    private readonly AppDbContext _dbContext = TestHelpers.BuildTestDbContext();

    private readonly IApplicationUserRepository _repository;

    private int _accountId;
    private Respawner _respawner;
    private int _userId;

    public AppUserRepositoryTests()
    {
        _repository = new AppUserRepository(_dbContext);
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

        var registrationRepository = new RegistrationRepository(_dbContext);
        var result = await TestHelpers.SetUpBaseRecords(registrationRepository);
        _accountId = result.Item1.Id;
        _userId = result.Item2.Id;
    }

    public async Task DisposeAsync()
    {
        using var conn = _dbContext.Database.GetDbConnection();
        await conn.OpenAsync();
        await _respawner.ResetAsync(conn);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser()
    {
        var result = await _repository.GetUserByIdAsync(_userId);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserByEmailHashAsync_ShouldReturnUser()
    {
        var user = await _repository.GetUserByIdAsync(_userId);

        var result = await _repository.GetUserByEmail(user.EmailHash);
        result.Should().NotBeNull();
    }
}