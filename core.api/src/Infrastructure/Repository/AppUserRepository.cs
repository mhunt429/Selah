using Microsoft.EntityFrameworkCore;
using Domain.Models.Entities.ApplicationUser;
using Infrastructure.Repository.Interfaces;

namespace Infrastructure.Repository;

public class AppUserRepository(AppDbContext appDbContext) : IApplicationUserRepository
{
    public async Task<ApplicationUserEntity?> GetUserByIdAsync(int id)
    {
        return await appDbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ApplicationUserEntity?> GetUserByEmail(string emailHash)
    {
        return await appDbContext.ApplicationUsers
            .FirstOrDefaultAsync(x => x.EmailHash == emailHash);
    }
}