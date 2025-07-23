using Domain.Models.Entities.Identity;

namespace Infrastructure.Repository.Interfaces;

public interface IUserSessionRepository
{
    Task<UserSessionEntity?> GetUserSessionAsync(int userId);
    
    Task IssueSession(UserSessionEntity userSession);
    
    Task RevokeSessionsByUser(int userId, bool autocommit);
    
}