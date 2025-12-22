using Domain.Models.Entities.Mailbox;
using Infrastructure.Repository.Interfaces;

namespace Application.Services;

public class UserMailboxService(IUserMailboxRepository userMailboxRepository)
{
    private readonly IUserMailboxRepository _userMailboxRepository = userMailboxRepository;

    public async Task<IEnumerable<UserMailboxEntity>> GetMessagesByUserId(int id)
    {
        return await _userMailboxRepository.GetMessagesByUserId(id);
    }

    public async Task<UserMailboxEntity?> GetMessagesByIdAndUserId(int id, int userId)
    {
        return await _userMailboxRepository.GetMessagesByIdAndUserId(id, userId);
    }

    public async Task SaveMessage(UserMailboxEntity message)
    {
        await _userMailboxRepository.InsertMessage(message);
    }

    public async Task DeleteMessage(int id, int userId)
    {
        await _userMailboxRepository.DeleteMessage(id, userId);
    }
    
    public async Task DeleteAllMessages(int userId)
    {
        await _userMailboxRepository.DeleteAllMessages(userId);
    }

    public async Task MarkMessageAsRead(int id, int userId)
    {
        await _userMailboxRepository.MarkMessageAsRead(id, userId);
    }
}