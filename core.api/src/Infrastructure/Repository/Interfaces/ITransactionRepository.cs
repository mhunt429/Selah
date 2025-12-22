using Domain.Models.DbUtils;
using Domain.Models.Entities.Transactions;

namespace Infrastructure.Repository.Interfaces;

public interface ITransactionRepository
{
    Task<DbOperationResult<TransactionEntity>> CreateTransaction(TransactionEntity transaction);

    Task UpdateTransaction(TransactionEntity transaction);

    Task<DbOperationResult<TransactionEntity?>> GetTransaction(int id, int userId);

    Task<DbOperationResult<IEnumerable<TransactionEntity>>> GetTransactionsByUser(
        int userId, int limit = 25, int cursor = 0, SortParameters? sortParameters = null);

    Task DeleteTransaction(int transactionId, int userId);

    Task<DbOperationResult<int>> AddTransactionsInBulk(IReadOnlyCollection<TransactionEntity> transactions);

    Task<DbOperationResult<int>> DeleteTransactionsInBulk(IReadOnlyCollection<string>? externalIds,
        int userId);

    Task<DbOperationResult<int>> UpdateTransactionsInBulk(IReadOnlyCollection<TransactionEntity> transactions,
        int userId);
}