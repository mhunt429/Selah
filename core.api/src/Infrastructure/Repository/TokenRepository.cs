using Domain.Models.Entities;
using Domain.Models.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class TokenRepository(IDbConnectionFactory dbConnectionFactory) : BaseRepository(dbConnectionFactory)
{
    public async Task SaveTokenAsync(TokenEntity token)
    {
        List<(string, object)> transactions = new List<(string, object)>()
        {
            (@"DELETE FROM token WHERE user_id = @user_id AND token_type = @token_type",
                new { user_id = token.UserId, token_type = token.TokenType }),
            (@"INSERT INTO token 
      (app_last_changed_by, user_id, token, token_type, created_at, expires_at) 
        VALUES (@app_last_changed_by, @user_id, @token, @token_type, @created_at, @expires_at) returning(id)",
                new
                {
                    app_last_changed_by = token.AppLastChangedBy,
                    user_id = token.UserId,
                    token = token.Token,
                    token_type = token.TokenType,
                    created_at = token.CreatedAt,
                    expires_at = token.ExpiresAt
                })
        };

        await PerformTransaction(transactions);
    }

    public async Task<TokenEntity?> GetTokenByUserId(int userId, string tokenType)
    {
        return await GetFirstOrDefaultAsync<TokenEntity>(@"SELECT * FROM token 
        WHERE user_id = @user_id 
        AND token_type = @token_type",
            new { user_id = userId, token_type = tokenType });
    }


    public async Task<TokenEntity?> GetByTokenHash(string tokenHash, string tokenType)
    {
        return await GetFirstOrDefaultAsync<TokenEntity>(@"SELECT * FROM token 
        WHERE token = @token 
        AND token_type = @token_type",
            new { token = tokenHash, token_type = tokenType });
    }

    public async Task DeleteTokenAsync(int userId, string tokenType)
    {
        await DeleteAsync("DELETE FROM token WHERE " +
                          "user_id = @user_id " +
                          "AND token_type = @token_type",
            new { user_id = userId, token_type = tokenType });
    }
}