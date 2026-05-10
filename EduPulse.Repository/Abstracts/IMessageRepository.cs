using EduPulse.Entities.Messages;

namespace EduPulse.Repository.Abstracts;

public interface IMessageRepository
{
    Task<List<Message>> GetInboxAsync(string schoolId, string userId);

    Task<List<Message>> GetSentAsync(string schoolId, string userId);

    Task<List<Message>> GetConversationAsync(
        string schoolId,
        string currentUserId,
        string otherUserId
    );

    Task<Message?> GetByIdAsync(string id);

    Task CreateAsync(Message message);

    Task UpdateAsync(Message message);
}