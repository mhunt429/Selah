using System.Data;
using Dapper;

namespace Infrastructure.Repository;

public class BaseRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public BaseRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<T> AddAsync<T>(string sql, object parameters)
    {
        using (IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync())
        {
            return await connection.ExecuteScalarAsync<T>(sql, parameters);
        }
    }

    public async Task<int> AddManyAsync<T>(string sql, IReadOnlyCollection<DynamicParameters> objectsToSave)
    {
        int rowsInserted = 0;
        using (IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync())
        {
            try
            {
                foreach (DynamicParameters obj in objectsToSave)
                {
                    await connection.ExecuteScalarAsync(sql, obj);
                    rowsInserted++;
                }

                return rowsInserted;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }

    public async Task<bool> DeleteAsync(string sql, object parameters)
    {
        using (IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync())
        {
            int recordsDeleted = await connection.ExecuteAsync(sql, parameters);
            return recordsDeleted > 0;
        }
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>(string sql, object parameters)
    {
        using (IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync())
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            return await connection.QueryAsync<T>(sql, parameters);
        }
    }

    public async Task<T> GetFirstOrDefaultAsync<T>(string sql, object parameters)
    {
        using (IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync())
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            return (await connection.QueryAsync<T>(sql, parameters)).FirstOrDefault();
        }
    }

    public async Task<bool> UpdateAsync(string sql, object parameters)
    {
        using (IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync())
        {
            var rowsUpdated = await connection.ExecuteAsync(sql, parameters);
            return rowsUpdated > 0;
        }
    }

    /// <summary>
    /// Use this one when inserting into multiple tables transactionally
    /// </summary>
    /// <param name="transactions"></param>
    public async Task PerformTransaction(List<(string, object)> transactions)
    {
        using (IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync())
        {
            using (var dbTransaction = connection.BeginTransaction())
            {
                try
                {
                    foreach (var transaction in transactions)
                    {
                        await connection.ExecuteAsync(transaction.Item1, transaction.Item2);
                    }

                    dbTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                    throw;
                }
            }
        }
    }

    /// <summary>
    /// Use this one if inserting into a single table transactionally
    /// </summary>
    /// <param name="transactions"></param>
    /// <param name="sql"></param>
    public async Task PerformTransaction<T>(IEnumerable<T> transactions, string sql)
    {
        using (IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync())
        {
            using (var dbTransaction = connection.BeginTransaction())
            {
                try
                {
                    foreach (var transaction in transactions)
                    {
                        await connection.ExecuteAsync(sql, transaction);
                    }

                    dbTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                    throw;
                }
            }
        }
    }
}