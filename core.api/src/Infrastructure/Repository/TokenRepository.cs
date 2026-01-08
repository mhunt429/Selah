using Domain.Models.Entities;
using Domain.Models.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class TokenRepository(AppDbContext dbContext)
{
    public async Task SaveTokenAsync(TokenEntity token)
    {
        var existingTokens = await dbContext.Tokens
            .Where(x => x.UserId == token.UserId && x.TokenType == token.TokenType)
            .ToListAsync();
        
        if (existingTokens.Any())
        {
            dbContext.Tokens.RemoveRange(existingTokens);
        }

        var newToken = new TokenEntity
        {
            UserId = token.UserId,
            Token = token.Token,
            TokenType = token.TokenType,
            ExpiresAt = token.ExpiresAt,
            CreatedAt = token.CreatedAt
        };

        await dbContext.Tokens.AddAsync(newToken);
        await dbContext.SaveChangesAsync();
    }

    public async Task<TokenEntity?> GetTokenByUserId(int userId, string tokenType)
    {
        return await dbContext.Tokens.FirstOrDefaultAsync(x => x.UserId == userId && x.TokenType == tokenType);
    }


    public async Task<TokenEntity?> GetByTokenHash(string tokenHash, string tokenType)
    {
        return await dbContext.Tokens.FirstOrDefaultAsync(x => x.Token == tokenHash && x.TokenType == tokenType);
    }

    public async Task DeleteTokenAsync(int userId, string tokenType)
    {
        await dbContext.Tokens.Where(x => x.UserId == userId && x.TokenType == tokenType)
            .ExecuteDeleteAsync();
    }
}