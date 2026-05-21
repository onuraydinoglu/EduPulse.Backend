using EduPulse.Entities.Messages;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EduPulse.Repository.Concretes;

public class MessageRepository : IMessageRepository
{
    private readonly IMongoCollection<Message> _messages;

    public MessageRepository(MongoDbContext context)
    {
        _messages = context.Messages;
    }

    public async Task<List<Message>> GetInboxAsync(string schoolId, string userId)
    {
        return await _messages
            .Find(x =>
                x.SchoolId == schoolId &&
                x.ReceiverUserId == userId &&
                !x.IsDeletedByReceiver
            )
            .SortByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<Message>> GetSentAsync(string schoolId, string userId)
    {
        return await _messages
            .Find(x =>
                x.SchoolId == schoolId &&
                x.SenderUserId == userId &&
                !x.IsDeletedBySender
            )
            .SortByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<Message>> GetConversationAsync(
        string schoolId,
        string currentUserId,
        string otherUserId
    )
    {
        return await _messages
            .Find(x =>
                x.SchoolId == schoolId &&
                (
                    (
                        x.SenderUserId == currentUserId &&
                        x.ReceiverUserId == otherUserId &&
                        !x.IsDeletedBySender
                    )
                    ||
                    (
                        x.SenderUserId == otherUserId &&
                        x.ReceiverUserId == currentUserId &&
                        !x.IsDeletedByReceiver
                    )
                )
            )
            .SortBy(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<Message>> GetByGroupIdAsync(string schoolId, string groupId)
    {
        return await _messages
            .Find(x =>
                x.SchoolId == schoolId &&
                x.GroupId == groupId
            )
            .ToListAsync();
    }

    public async Task<Message?> GetByIdAsync(string id)
    {
        return await _messages
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Message message)
    {
        if (string.IsNullOrWhiteSpace(message.Id))
        {
            message.Id = ObjectId.GenerateNewId().ToString();
        }

        message.CreatedDate = DateTime.UtcNow;
        message.UpdatedDate = null;

        await _messages.InsertOneAsync(message);
    }

    public async Task UpdateAsync(Message message)
    {
        message.UpdatedDate = DateTime.UtcNow;

        await _messages.ReplaceOneAsync(x => x.Id == message.Id, message);
    }
}