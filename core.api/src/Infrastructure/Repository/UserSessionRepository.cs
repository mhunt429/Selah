using Domain.ApiContracts;
using Domain.Models.Entities.ApplicationUser;
using Microsoft.EntityFrameworkCore;
using Domain.Models.Entities.Identity;
using Infrastructure.Repository.Interfaces;

namespace Infrastructure.Repository;

public class UserSessionRepository : BaseRepository, IUserSessionRepository
{
    public UserSessionRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
    {
    }

    public async Task<UserSessionEntity?> GetUserSessionAsync(int userId)
    {
        return await GetFirstOrDefaultAsync<UserSessionEntity>("select * from user_session where user_id = @user_id",
            new { user_id = userId });
    }

    /// <summary>
    /// Revoke any existing sessions for a given user before issuing a new one
    /// </summary>
    /// <param name="userSession"></param>
    public async Task IssueSession(UserSessionEntity userSession)
    {
        await RevokeSessionsByUser(userSession.UserId);

        string sql =
            @"INSERT INTO user_session(id, user_id, issued_at, 
                         expires_at, 
                         app_last_changed_by) 
                         VALUES (@id, @user_id, @issued_at, @expires_at, @app_last_changed_by)";

        await AddAsync<int>(sql, new
        {
            id = userSession.Id,
            user_id = userSession.UserId,
            issued_at = userSession.IssuedAt,
            expires_at = userSession.ExpiresAt,
            app_last_changed_by = userSession.AppLastChangedBy
        });
    }

    public async Task RevokeSessionsByUser(int userId)
    {
        await DeleteAsync("DELETE FROM user_session WHERE user_id = @user_id", new { user_id = userId });
    }

    public async Task<int> GetActiveSessions()
    {
        string sql = "SELECT COUNT(*) FROM user_session WHERE expires_at > @currentDate";

        return await GetFirstOrDefaultAsync<int>(sql, new { currentDate = DateTimeOffset.UtcNow });
    }

    public async Task<ApplicationUserEntity> GetUserByActiveSessionId(Guid sessionId)
    {
        string sql = @"SELECT u.*
            FROM app_user u
            INNER JOIN user_session us on u.id = us.user_id
            where us.id = @id
            AND expires_at > now()";
        
        return await GetFirstOrDefaultAsync<ApplicationUserEntity>(sql, new { id = sessionId });
    }
}