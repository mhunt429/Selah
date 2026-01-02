using Domain.ApiContracts;
using Domain.Models.Entities.ApplicationUser;
using Microsoft.EntityFrameworkCore;
using Domain.Models.Entities.Identity;
using Infrastructure.Repository.Interfaces;

namespace Infrastructure.Repository;

public class UserSessionRepository(AppDbContext dbContext)
    : IUserSessionRepository
{
    public async Task<UserSessionEntity?> GetUserSessionAsync(int userId)
    {
        return await dbContext.UserSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    /// <summary>
    /// Revoke any existing sessions for a given user before issuing a new one
    /// </summary>
    /// <param name="userSession"></param>
    public async Task IssueSession(UserSessionEntity userSession)
    {
        dbContext.UserSessions.Remove(userSession);
        
        await dbContext.UserSessions.AddAsync(userSession);
        
        await dbContext.SaveChangesAsync();
    }

    public async Task RevokeSessionsByUser(int userId)
    {
        await dbContext.UserSessions.Where(x => x.UserId == userId).ExecuteDeleteAsync();
    }

    public async Task<int> GetActiveSessions()
    {
        return await dbContext.UserSessions.Where(x => x.ExpiresAt == DateTimeOffset.UtcNow).CountAsync();
    }
}