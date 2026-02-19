public interface IRecurringTransactionRepository
{
    Task<RecurringTransactionEntity> CreateRecurringTransaction(RecurringTransactionEntity recurringTransaction);

    Task<RecurringTransactionEntity?> GetRecurringTransactionById(int id, int userId);

    Task<int> DeleteRecurringTransaction(int id, int userId);
}