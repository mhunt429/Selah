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

    public async Task<IEnumerable<RecurringTransactionEntity>> GetRecurringTransactionsByUserId(int userId)
    {
        return await dbContext.RecurringTransactions.Where(x => x.UserId == userId).ToListAsync();
    }

    public async Task<int> DeleteRecurringTransaction(int id, int userId)
    {
        return await dbContext.RecurringTransactions.Where(x => x.Id == id && x.UserId == userId).ExecuteDeleteAsync();
    }

    public async Task<int> UpdateRecurringTransaction(RecurringTransactionEntity recurringTransaction)
    {
        return await dbContext.RecurringTransactions.Where(x => x.Id == recurringTransaction.Id && x.UserId == recurringTransaction.UserId)
        .ExecuteUpdateAsync(x => 
        x.SetProperty(y => y.AverageAmount, recurringTransaction.AverageAmount)
        .SetProperty(y => y.LastAmount, recurringTransaction.LastAmount)
        .SetProperty(y => y.FirstDate, recurringTransaction.FirstDate)
        .SetProperty(y => y.LastDate, recurringTransaction.LastDate)
        .SetProperty(y => y.PredictedNextDate, recurringTransaction.PredictedNextDate)
        .SetProperty(y => y.Frequency, recurringTransaction.Frequency));
    }
}