using Domain.Models.DbUtils;
using Domain.Models.Entities.Transactions;

namespace Infrastructure.Repository.Interfaces;

public interface ITransactionRepository
{
    Task<TransactionEntity> CreateTransaction(TransactionEntity transaction);

    Task UpdateTransaction(TransactionEntity transaction);

    Task<TransactionEntity?> GetTransaction(int id, int userId);

    Task<IEnumerable<TransactionEntity>> GetTransactionsByUser(
        int userId, int limit = 25, int cursor = 0, SortParameters? sortParameters = null);

    Task DeleteTransaction(int transactionId, int userId);

    Task AddTransactionsInBulk(IReadOnlyCollection<TransactionEntity> transactions);

    Task DeleteTransactionsInBulk(List<string> externalIds,
        int userId);

    Task UpdateTransactionsInBulk(IReadOnlyCollection<TransactionEntity> transactions,
        int userId);
}