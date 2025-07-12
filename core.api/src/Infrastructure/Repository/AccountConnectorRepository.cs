using Dapper;
using Domain.Constants;
using Domain.Models;
using Domain.Models.Entities.AccountConnector;
using Infrastructure.Extensions;

namespace Infrastructure.Repository;

public class AccountConnectorRepository: IAccountConnectorRepository
{
    private readonly AppDbContext _dbContext;

    public AccountConnectorRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    /// <summary>
    /// Insert into account_connector upon successful connection through Plaid or Finicity
    /// </summary>
    public async Task<DbOperationResult> InsertAccountConnectorRecord(AccountConnectorEntity account)
    {
        try
        {
            await _dbContext.AccountConnectors.AddAsync(account);
            await _dbContext.SaveChangesAsync();
            return new DbOperationResult(status: ResultStatus.Success, null);
        }

        catch (Exception ex)
        {
            return new DbOperationResult(status: ResultStatus.Failed, ex + ex.StackTrace);
        }
    }
}