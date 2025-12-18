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
        int userId, int limit = 25, int cursor = 0,  SortParameters? sortParameters = null)
    {
        IQueryable<TransactionEntity> query = dbContext.Transactions
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.Id > cursor);

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
        using (var dbTransaction = await dbContext.Database.BeginTransactionAsync())
        {
            try
            {
                var transactionToDelete  = await dbContext.Transactions
                    .Include(t => t.LineItems)
                    .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId);
                if (transactionToDelete != null )
                {
                    dbContext.Transactions.Remove(transactionToDelete);
                    
                    await dbContext.SaveChangesAsync();
                    await dbTransaction.CommitAsync();
                }
            }

            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
            }
        }
    }
}