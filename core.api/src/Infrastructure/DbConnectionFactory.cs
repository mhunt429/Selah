using System.Threading.Tasks;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Npgsql;
namespace Infrastructure
{
    public interface IDbConnectionFactory
    {
        public Task<IDbConnection> CreateConnectionAsync();
    }

    [ExcludeFromCodeCoverage]
    public class SelahDbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SelahDbConnectionFactory(string connectionString)
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



