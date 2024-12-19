using Dating.Data.Entities;
using Dating.Shared;

namespace Dating.Data.IRepositories;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message?> GetMessage(int id);
    Task<PageList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
    Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientCurrentUserName);
    Task<bool> SaveAllChanges();
}