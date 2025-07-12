using Domain.Models.Entities.ApplicationUser;

namespace Infrastructure.Repository.Interfaces;

public interface IApplicationUserRepository
{
    Task<ApplicationUserEntity?> GetUserByIdAsync(Guid userId);

    Task<ApplicationUserEntity?> GetUserByEmail(string emailHash);
}