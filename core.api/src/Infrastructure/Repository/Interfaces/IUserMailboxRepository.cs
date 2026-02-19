using Domain.Models.Entities.Mailbox;

namespace Infrastructure.Repository.Interfaces;

public interface IUserMailboxRepository
{
    Task<IEnumerable<UserMailboxEntity>> GetMessagesByUserId(int userId);

    Task<UserMailboxEntity?> GetMessagesByIdAndUser(int id, int userId);

    Task InsertMessage(UserMailboxEntity mailbox);

    Task DeleteMessage(int id, int userId);

    Task DeleteAllMessages(int userId);

    Task MarkMessageAsRead(int id, int userId);
}