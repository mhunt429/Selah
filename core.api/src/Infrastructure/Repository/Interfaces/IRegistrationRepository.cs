using Domain.Models.Entities.ApplicationUser;

namespace Infrastructure.Repository.Interfaces;

public interface IRegistrationRepository
{
    /// <summary>
    /// Returns a tuple of the accountId and the userId since they are auto-incremented from the database. User creates an account,
    /// gets, a token on success
    /// </summary>
    Task<(int, int)> RegisterAccount(ApplicationUserEntity user);
}