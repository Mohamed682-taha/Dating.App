using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dating.Data.Entities;
using Dating.Data.IRepositories;
using Dating.Repository.Data;
using Dating.Shared;
using Microsoft.EntityFrameworkCore;

namespace Dating.Repository;

public class MessageRepository(DatingDbContext context, IMapper mapper) : IMessageRepository
{
    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    public async Task<Message?> GetMessage(int id)
    {
        return await context.Messages.FindAsync(id);
    }

    public async Task<PageList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = context.Messages
            .OrderByDescending(m => m.MessageSentDate)
            .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(m => m.Recipient.UserName == messageParams.UserName && m.RecipientDeleted == false),
            "Outbox" => query.Where(m => m.Sender.UserName == messageParams.UserName && m.SenderDeleted == false),
            _ => query.Where(m =>
                m.Recipient.UserName == messageParams.UserName && m.DateRead == null && m.RecipientDeleted == false)
        };

        var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);
        return await PageList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
    {
        var messages = await context.Messages
            .Where(m => (m.RecipientUserName == currentUserName && m.RecipientDeleted == false &&
                         m.SenderUserName == recipientUserName) ||
                        (m.SenderUserName == currentUserName && m.SenderDeleted == false &&
                         m.RecipientUserName == recipientUserName))
            .OrderBy(m => m.MessageSentDate)
            .ProjectTo<MessageDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        var unreadMessage = messages
            .Where(m => m.DateRead == null && m.RecipientUserName == currentUserName)
            .ToList();
        if (unreadMessage.Count != 0)
        {
            unreadMessage.ForEach(x => x.DateRead = DateTime.UtcNow);
            await context.SaveChangesAsync();
        }

        return messages;
    }

    public async Task<bool> SaveAllChanges()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void AddGroup(Group group)
    {
        context.Groups.Add(group);
    }

    public void RemoveConnection(Connection connection)
    {
        context.Connections.Remove(connection);
    }

    public async Task<Connection?> GetConnection(string connectionId)
    {
        return await context.Connections.FindAsync(connectionId);
    }

    public async Task<Group?> GetMessageGroup(string groupName)
    {
        return await context.Groups
            .Include(x => x.Connections)
            .FirstOrDefaultAsync(x => x.Name == groupName);
    }

    public async Task<Group?> GetGroupForConnection(string connectionId)
    {
        return await context.Groups
            .Include(x => x.Connections)
            .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
    }
}