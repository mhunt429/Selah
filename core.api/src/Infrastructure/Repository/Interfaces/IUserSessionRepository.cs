using Domain.Models.Entities.Identity;

namespace Infrastructure.Repository.Interfaces;

public interface IUserSessionRepository
{
    Task<UserSessionEntity?> GetUserSessionAsync(Guid userId);
    
    Task IssueSession(UserSessionEntity userSession);
    
    Task RevokeSessionsByUser(Guid userId, bool autocommit);
    
}