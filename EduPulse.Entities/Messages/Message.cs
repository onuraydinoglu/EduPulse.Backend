using EduPulse.Entities.Common;

namespace EduPulse.Entities.Messages;

public class Message : BaseEntity
{
    public string SchoolId { get; set; } = null!;

    public string SenderUserId { get; set; } = null!;

    public string ReceiverUserId { get; set; } = null!;

    public string? GroupId { get; set; }

    public string? ReceiverGroupName { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public bool IsRead { get; set; } = false;

    public bool IsDeletedBySender { get; set; } = false;

    public bool IsDeletedByReceiver { get; set; } = false;
}