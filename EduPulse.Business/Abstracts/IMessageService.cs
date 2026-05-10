using EduPulse.DTOs.Common;
using EduPulse.DTOs.Messages;

namespace EduPulse.Business.Abstracts;

public interface IMessageService
{
    Task<Result<List<MessageUserListDto>>> GetMessageUsersAsync(
        string? currentUserId,
        string? currentRoleName,
        string? currentSchoolId
    );

    Task<Result<List<MessageListDto>>> GetInboxAsync(
        string? currentUserId,
        string? currentSchoolId
    );

    Task<Result<List<MessageListDto>>> GetSentAsync(
        string? currentUserId,
        string? currentSchoolId
    );

    Task<Result<List<MessageListDto>>> GetConversationAsync(
        string otherUserId,
        string? currentUserId,
        string? currentSchoolId
    );

    Task<Result> SendAsync(
        CreateMessageDto dto,
        string? currentUserId,
        string? currentSchoolId
    );

    Task<Result> MarkAsReadAsync(
        string id,
        string? currentUserId,
        string? currentSchoolId
    );

    Task<Result> DeleteAsync(
        string id,
        string? currentUserId,
        string? currentSchoolId
    );
}