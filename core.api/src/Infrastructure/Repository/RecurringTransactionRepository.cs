using Infrastructure;
using Microsoft.EntityFrameworkCore;

public class RecurringTransactionRepository(AppDbContext dbContext) : IRecurringTransactionRepository
{
    public async Task<RecurringTransactionEntity> CreateRecurringTransaction(RecurringTransactionEntity recurringTransaction)
    {
        await dbContext.RecurringTransactions.AddAsync(recurringTransaction);
        await dbContext.SaveChangesAsync();
        return recurringTransaction;
    }

    public async Task<RecurringTransactionEntity?> GetRecurringTransactionById(int id, int userId)
    {
        return await dbContext.RecurringTransactions.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
    }

    public async Task<int> DeleteRecurringTransaction(int id, int userId)
    {
        return await dbContext.RecurringTransactions.Where(x => x.Id == id && x.UserId == userId).ExecuteDeleteAsync();
    }
}