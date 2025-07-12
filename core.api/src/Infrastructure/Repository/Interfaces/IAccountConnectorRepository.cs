using Domain.Models;
using Domain.Models.Entities.AccountConnector;

namespace Infrastructure.Repository;

public interface IAccountConnectorRepository
{
    Task<DbOperationResult> InsertAccountConnectorRecord(AccountConnectorEntity account);
}