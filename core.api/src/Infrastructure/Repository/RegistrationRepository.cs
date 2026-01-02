using Domain.Models.Entities.ApplicationUser;
using Domain.Models.Entities.UserAccount;
using Infrastructure.Repository.Interfaces;

namespace Infrastructure.Repository;

public class RegistrationRepository(AppDbContext dbContext) : IRegistrationRepository
{
    public async Task<(int, int)> RegisterAccount(ApplicationUserEntity user)
    {
        await dbContext.ApplicationUsers.AddAsync(user);
        await dbContext.SaveChangesAsync();

        return (user.AccountId, user.Id);
    }
}