using Domain.Models.Entities.AccountConnector;
using Infrastructure.Repository.Interfaces;

namespace Infrastructure.Repository;

public class AccountConnectorRepository : IAccountConnectorRepository
{
    private readonly AppDbContext _dbContext;

    public AccountConnectorRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    /// <summary>
    ///     Insert into account_connector upon successful connection through Plaid or Finicity
    /// </summary>
    public async Task<int> InsertAccountConnectorRecord(AccountConnectorEntity account)
    {
        await _dbContext.AccountConnectors.AddAsync(account);
        await _dbContext.SaveChangesAsync();
        return account.Id;
    }
}