using Microsoft.EntityFrameworkCore;
using Domain.Constants;
using Domain.Models.Entities.ApplicationUser;
using Infrastructure.Repository.Interfaces;

namespace Infrastructure.Repository;

public class AppUserRepository : IApplicationUserRepository
{
    private readonly AppDbContext _appDbContext;

    public AppUserRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<ApplicationUserEntity?> GetUserByIdAsync(int id)
    {
        return await _appDbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ApplicationUserEntity?> GetUserByEmail(string emailHash)
    {
        return await _appDbContext.ApplicationUsers
            .FirstOrDefaultAsync(x => x.EmailHash == emailHash);
    }
}