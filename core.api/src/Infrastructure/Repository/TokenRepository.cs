using Domain.Models.Entities;
using Domain.Models.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class TokenRepository: BaseRepository
{

  public TokenRepository(IDbConnectionFactory dbConnectionFactory): base(dbConnectionFactory)
  {

  }

  public async Task<int> CreateTokenAsync(TokenEntity token)
  { 
    var sql = @"INSERT INTO token 
    (app_last_changed_by, user_id, token, token_type, created_at, expires_at) 
VALUES (@app_last_changed_by, @user_id, @token, @token_type, @created_at, @expires_at) returning(id)";
    return await AddAsync<int>(sql, new
    {
      app_last_changed_by = token.AppLastChangedBy,
      user_id = token.UserId,
      token = token.Token,
      token_type = token.TokenType,
      created_at = token.CreatedAt,
      expires_at = token.ExpiresAt
    });
  }

  public async Task<TokenEntity?> GetTokenByUserId(int userId, string tokenType )
  {

    return await GetFirstOrDefaultAsync<TokenEntity>(@"SELECT * FROM token 
        WHERE user_id = @user_id 
        AND token_type = @token_type",
      new { user_id = userId, token_type = tokenType });
  }

  public async Task DeleteTokenAsync(int userId, string tokenType)
  {
   await DeleteAsync("DELETE FROM token WHERE " +
                     "user_id = @user_id " +
                     "AND token_type = @token_type", 
     new {user_id = userId, token_type = tokenType });
  }
}