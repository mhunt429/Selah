using Dapper;

namespace Infrastructure.Repository;

public interface IBaseRepository
{
    Task<T> AddAsync<T>(string sql, object parameters);

    Task<int> AddManyAsync<T>(string sql, IReadOnlyCollection<DynamicParameters> objectsToSave);

    Task<bool> DeleteAsync(string sql, object parameters);

    Task<IEnumerable<T>> GetAllAsync<T>(string sql, object parameters);

    Task<T> GetFirstOrDefaultAsync<T>(string sql, object parameters);

    Task<bool> UpdateAsync(string sql, object parameters);

    Task PerformTransaction(List<(string, object)> transactions);

    Task PerformTransaction(IEnumerable<DynamicParameters> transactions, string sql);
}