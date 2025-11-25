using System.Data.Common;
using Npgsql;
using Respawn;
using Respawn.Graph;

namespace IntegrationTests.Helpers;

public class DatabaseFixture : IAsyncLifetime
{
    private DbConnection _connection;
    public Respawner Respawner { get; private set; }

    public async Task InitializeAsync()
    {
        _connection = new NpgsqlConnection(TestHelpers.TestConnectionString);
        await _connection.OpenAsync();

        // Initialize Respawner once
        Respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = new[] { new Table("flyway_schema_history") }
        });

        await Respawner.ResetAsync(_connection);
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await Respawner.ResetAsync(_connection);
    }
}