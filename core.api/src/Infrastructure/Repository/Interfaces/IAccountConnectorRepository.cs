using Domain.Models.Entities.AccountConnector;

namespace Infrastructure.Repository.Interfaces;

public interface IAccountConnectorRepository
{
    Task<int> InsertAccountConnectorRecord(AccountConnectorEntity account);

    Task<IEnumerable<AccountConnectorEntity>> GetAccountConnectorRecords(int userId);
}