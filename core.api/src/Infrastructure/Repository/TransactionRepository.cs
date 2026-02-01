using Domain.Models.DbUtils;
using Domain.Models.Entities.Transactions;
using Domain.Models.Plaid;
using Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class TransactionRepository(AppDbContext dbContext) : ITransactionRepository
{
    public async Task<DbOperationResult<IEnumerable<TransactionLineItemEntity>>> GetTransactionLineItems(
        int transactionId)
    {
        List<TransactionLineItemEntity> lineItems = await dbContext.TransactionLineItems
            .AsNoTracking()
            .Where(x => x.TransactionId == transactionId)
            .ToListAsync();

        return new DbOperationResult<IEnumerable<TransactionLineItemEntity>>()
        {
            Status = ResultStatus.Success,
            Data = lineItems
        };
    }

    public async Task<DbOperationResult<TransactionEntity>> CreateTransaction(
        TransactionEntity transaction)
    {
        try
        {
            dbContext.Transactions.Add(transaction);
            await dbContext.SaveChangesAsync();

            return new DbOperationResult<TransactionEntity>
            {
                Status = ResultStatus.Success,
                Data = transaction
            };
        }
        catch (Exception ex)
        {
            return new DbOperationResult<TransactionEntity>
            {
                Status = ResultStatus.Failure,
                ErrorMessage = ex.Message + ex.StackTrace
            };
        }
    }

    public async Task UpdateTransaction(TransactionEntity transaction)
    {
        var existing = await dbContext.Transactions
            .FirstOrDefaultAsync(t => t.Id == transaction.Id);

        if (existing != null)
        {
            dbContext.Entry(existing).CurrentValues.SetValues(transaction);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<DbOperationResult<TransactionEntity?>> GetTransaction(int id, int userId)
    {
        TransactionEntity? transaction = await dbContext.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        return new DbOperationResult<TransactionEntity?>()
        {
            Status = ResultStatus.Success,
            Data = transaction
        };
    }

    public async Task<DbOperationResult<IEnumerable<TransactionEntity>>> GetTransactionsByUser(
        int userId, int limit = 25, int cursor = 0, SortParameters? sortParameters = null)
    {
        IQueryable<TransactionEntity> query = dbContext.Transactions
            .AsNoTracking()
            .Where(x => x.UserId == userId);

        query = sortParameters?.SortColumn switch
        {
            "id" => sortParameters.SortDirection == "DESC"
                ? query.OrderByDescending(t => t.Id)
                : query.OrderBy(t => t.Id),

            "date" => sortParameters.SortDirection == "DESC"
                ? query.OrderByDescending(t => t.TransactionDate)
                : query.OrderBy(t => t.TransactionDate),

            "amount" => sortParameters.SortDirection == "DESC"
                ? query.OrderByDescending(t => t.Amount)
                : query.OrderBy(t => t.Amount),

            _ => query.OrderBy(t => t.Id)
        };

        List<TransactionEntity> data = await query.Take(limit).ToListAsync();

        return new DbOperationResult<IEnumerable<TransactionEntity>>
        {
            Status = ResultStatus.Success,
            Data = data
        };
    }

    public async Task DeleteTransaction(int transactionId, int userId)
    {
        await dbContext.Transactions
            .Include(t => t.LineItems)
            .Where(t => t.Id == transactionId && t.UserId == userId)
            .ExecuteDeleteAsync();
    }


    public async Task<DbOperationResult<int>> AddTransactionsInBulk(IReadOnlyCollection<TransactionEntity> transactions)
    {
        await dbContext.Transactions.AddRangeAsync(transactions);
        await dbContext.SaveChangesAsync();

        return new DbOperationResult<int>()
        {
            Status = ResultStatus.Success,
            Data = transactions.Count()
        };
    }

    public async Task<DbOperationResult<int>> DeleteTransactionsInBulk(IReadOnlyCollection<string>? externalIds,
        int userId)
    {
        if (externalIds == null || externalIds.Count == 0)
        {
            return new DbOperationResult<int>()
            {
                Status = ResultStatus.Success,
                Data = 0
            };
        }

        var deletedTransactions = await dbContext.Transactions
            .Where(x =>
                x.UserId == userId &&
                !string.IsNullOrEmpty(x.ExternalTransactionId) &&
                externalIds.Contains(x.ExternalTransactionId))
            .ExecuteDeleteAsync();

        return new DbOperationResult<int>()
        {
            Status = ResultStatus.Success,
            Data = deletedTransactions
        };
        ;
    }

    public async Task<DbOperationResult<int>> UpdateTransactionsInBulk(
        IReadOnlyCollection<TransactionEntity> transactions, int userId)
    {
        await dbContext.Transactions.Where(x =>
                x.UserId == userId &&
                transactions.Select(t => t.ExternalTransactionId)
                    .Contains(x.ExternalTransactionId)
            )
            .ExecuteDeleteAsync();

        await dbContext.Transactions.AddRangeAsync(transactions);
        await dbContext.SaveChangesAsync();


        return new DbOperationResult<int>
        {
            Status = ResultStatus.Success,
        };
    }

    public async Task<decimal> GetExpenseTransactionTotals(int userId, DateTimeOffset start, DateTimeOffset end)
    {
        return await dbContext.Transactions
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.TransactionDate >= start && x.TransactionDate <= end)
            .SumAsync(x => x.Amount);
    }
}