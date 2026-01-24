using Domain.Models.Entities.Transactions;
using Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class TransactionCategoryRepository(AppDbContext dbContext) : ITransactionCategoryRepository
{
    public async Task<int> AddCategoryAsync(TransactionCategoryEntity categoryEntity)
    {
        await dbContext.TransactionCategories.AddAsync(categoryEntity);
        await dbContext.SaveChangesAsync();

        return categoryEntity.Id;
    }

    public async Task<TransactionCategoryEntity?> GetCategoryByIdAndUserAsync(int id, int userId)
    {
        return await dbContext.TransactionCategories.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
    }

    public async Task<IReadOnlyCollection<TransactionCategoryEntity>> GetAllCategoriesByUser(int userId)
    {
        return await dbContext.TransactionCategories.AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<int> DeleteCategoryById(int id, int userId)
    {
        return await dbContext.TransactionCategories.Where(x => x.Id == id && x.UserId == userId)
            .ExecuteDeleteAsync();
    }

    public async Task<int> UpdateCategoryAsync(int id, int useId, string name)
    {
        return await dbContext.TransactionCategories.Where(x => x.Id == id && x.UserId == useId)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(s => s.CategoryName, name));
    }
}