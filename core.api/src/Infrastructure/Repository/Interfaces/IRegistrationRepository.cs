using Domain.Models.Entities.ApplicationUser;
using Domain.Models.Entities.UserAccount;

namespace Infrastructure.Repository;

public interface IRegistrationRepository
{
    /// <summary>
    /// Returning simply userId due to the transactional nature of this. User creates an account,
    /// gets, a token on success
    /// </summary>
    Task<Guid> RegisterAccount(UserAccountEntity userAccount, ApplicationUserEntity user);
}