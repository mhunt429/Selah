using Domain.Models;
using Domain.Models.Entities.AccountConnector;

namespace Infrastructure.Repository;

public interface IAccountConnectorRepository
{
    Task<int> InsertAccountConnectorRecord(AccountConnectorEntity account);
}