using Domain.ApiContracts.Mailbox;
using Domain.Models.Entities.Mailbox;
using Infrastructure.Repository.Interfaces;

namespace Application.Services;

public class UserMailboxService(IUserMailboxRepository userMailboxRepository)
{
    public async Task<IEnumerable<MailboxResponse>> GetMessagesByUserId(int id)
    {
        var dbMessages = await userMailboxRepository.GetMessagesByUserId(id);

        return dbMessages.Select(x => new MailboxResponse
        {
            Id = x.Id,
            CreatedAt = x.OriginalInsert,
            Unread = !x.HasSeen,
            MessageBody = x.MessageBody,
            MessageKey = x.MessageKey,
        });
    }

    public async Task<MailboxResponse?> GetMessagesByIdAndUserId(int id, int userId)
    {
        var dbMessage = await userMailboxRepository.GetMessagesByIdAndUserId(id, userId);

        if (dbMessage == null) return null;

        return new MailboxResponse
        {
            Id = dbMessage.Id,
            CreatedAt = dbMessage.OriginalInsert,
            Unread = !dbMessage.HasSeen,
            MessageBody = dbMessage.MessageBody,
            MessageKey = dbMessage.MessageKey,
        };
    }
    

    public async Task DeleteMessage(int id, int userId)
    {
        await userMailboxRepository.DeleteMessage(id, userId);
    }

    public async Task DeleteAllMessages(int userId)
    {
        await userMailboxRepository.DeleteAllMessages(userId);
    }

    public async Task MarkMessageAsRead(int id, int userId)
    {
        await userMailboxRepository.MarkMessageAsRead(id, userId);
    }
}