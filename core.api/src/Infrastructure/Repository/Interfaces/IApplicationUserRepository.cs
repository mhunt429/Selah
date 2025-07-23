using Domain.Models.Entities.ApplicationUser;

namespace Infrastructure.Repository.Interfaces;

public interface IApplicationUserRepository
{
    Task<ApplicationUserEntity?> GetUserByIdAsync(int userId);

    Task<ApplicationUserEntity?> GetUserByEmail(string emailHash);
}