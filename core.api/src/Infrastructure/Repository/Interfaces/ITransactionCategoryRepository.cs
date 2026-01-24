using Domain.Models.Entities.Transactions;

namespace Infrastructure.Repository.Interfaces;

public interface ITransactionCategoryRepository
{
    Task<int> AddCategoryAsync(TransactionCategoryEntity categoryEntity);

    Task<TransactionCategoryEntity?> GetCategoryByIdAndUserAsync(int id, int userId);

    Task<IReadOnlyCollection<TransactionCategoryEntity>> GetAllCategoriesByUser(int userId);

    Task<int> DeleteCategoryById(int id, int userId);

    Task<int> UpdateCategoryAsync(int id, int useId, string name);
}