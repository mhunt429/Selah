using FluentAssertions;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Respawn;
using Respawn.Graph;

namespace Infrastructure.IntegrationTests.Repository;

public class RegistrationRepositoryTests : IAsyncLifetime
{
    private readonly AppDbContext _dbContext = TestHelpers.BuildTestDbContext();
    private readonly RegistrationRepository _repository;
    private Respawner _respawner;
    private int _userId;


    public RegistrationRepositoryTests()
    {
        _repository = new RegistrationRepository(TestHelpers.BuildTestDbContext());
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
    }

    public async Task DisposeAsync()
    {
        using var conn = _dbContext.Database.GetDbConnection();
        await conn.OpenAsync();
        await _respawner.ResetAsync(conn);
    }

    [Fact]
    public async Task Register_ShouldSaveAccountAndUserRecord()
    {
        var result = await TestHelpers.SetUpBaseRecords(_repository);
        _userId = result.Item2.Id;
        result.Should().NotBeNull();
        result.Item2.Id.Should().Be(_userId);
    }
}