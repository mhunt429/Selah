using Domain.Models.DbUtils;
using Domain.Models.Entities.Transactions;

namespace Infrastructure.Repository;

public class TransactionRepository(IDbConnectionFactory dbConnectionFactory) : BaseRepository(dbConnectionFactory)
{
    public async Task<DbOperationResult<IEnumerable<TransactionLineItemEntity>>> GetTransactionLineItems(
        int transactionId)
    {
        var sql = "SELECT * FROM transaction_line_item where transaction_id = @transaction_id";

        var data = await GetAllAsync<TransactionLineItemEntity>(sql, new { transaction_id = transactionId });

        return new DbOperationResult<IEnumerable<TransactionLineItemEntity>>()
        {
            Status = ResultStatus.Success,
            Data = data
        };
    }

    public async Task<DbOperationResult<IEnumerable<TransactionEntity>>> GetTransactionsByUser(
        SortParameters sortParameters, int userId, int limit = 25, int cursor = 0)
    {
        if (!DbUtils.SortColumnMap.TryGetValue(sortParameters.SortColumn, out var sortColumn))
        {
            return new DbOperationResult<IEnumerable<TransactionEntity>>()
            {
                Status = ResultStatus.Failure,
                ErrorMessage = $"$Invalid sort column {sortParameters.SortColumn}"
            };
        }

        var sortDirection = DbUtils.NormalizeSortDirection(sortParameters.SortDirection);
        
        var sql = @"SELECT * FROM transaction 
         WHERE 
             user_id = @user_id AND id > @cursor  ORDER BY {sortColumn} {sortDirection} LIMIT @limit";

        var data = await GetAllAsync<TransactionEntity>(sql, new
        {
            user_id = userId,
            cursor,
            limit
        });

        return new DbOperationResult<IEnumerable<TransactionEntity>>
        {
            Status = ResultStatus.Success,
            Data = data
        };
    }
}