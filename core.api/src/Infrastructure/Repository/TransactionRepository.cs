using Domain.Models.DbUtils;
using Domain.Models.Entities.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class TransactionRepository(AppDbContext dbContext)
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

            dbContext.TransactionLineItems.AddRange(transaction.LineItems);

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

    public async Task<DbOperationResult<TransactionEntity?>> UpdateTransaction(TransactionEntity transaction)
    {
        dbContext.Transactions.Update(transaction);
        await dbContext.SaveChangesAsync();

        return new DbOperationResult<TransactionEntity?>
        {
            Status = ResultStatus.Success,
            Data = transaction
        };
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
        SortParameters sortParameters, int userId, int limit = 25, int cursor = 0)
    {
        IQueryable<TransactionEntity> query = dbContext.Transactions
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.Id > cursor);

        query = sortParameters.SortColumn switch
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
}