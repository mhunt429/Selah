using System.Data;
using Npgsql;

namespace Infrastructure
{
    public interface IDbConnectionFactory
    {
        public Task<IDbConnection> CreateConnectionAsync();
    }

    
    public class CortadoDbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public CortadoDbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}