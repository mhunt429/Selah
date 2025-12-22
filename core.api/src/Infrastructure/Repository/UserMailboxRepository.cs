using Domain.Models.Entities.Mailbox;
using Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class UserMailboxRepository(AppDbContext dbContext): IUserMailboxRepository
{
    public async Task<IEnumerable<UserMailboxEntity>> GetMessagesByUserId(int userId)
    {
        return await dbContext.UserMailboxes
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserMailboxEntity?> GetMessagesByIdAndUserId(int id, int userId)
    {
        return await dbContext.UserMailboxes
            .Where(x => x.Id == id && x.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task InsertMessage(UserMailboxEntity mailbox)
    {
        await dbContext.UserMailboxes.AddAsync(mailbox);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteMessage(int id, int userId)
    {
        var messageToDelete = await dbContext.UserMailboxes
            .Where(x => x.Id == id && x.UserId == userId)
            .FirstOrDefaultAsync();

        if (messageToDelete != null)
        {
            dbContext.UserMailboxes.Remove(messageToDelete);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteAllMessages(int userId)
    {
        var messagesToDelete = dbContext.UserMailboxes
            .Where(x => x.UserId == userId);
        dbContext.UserMailboxes.RemoveRange(messagesToDelete);
        await dbContext.SaveChangesAsync();
    }

    public async Task MarkMessageAsRead(int id, int userId)
    {
        await dbContext.UserMailboxes
            .Where(x => x.Id == id && x.UserId == userId && !x.HasSeen)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(x => x.HasSeen, true));
    }
}