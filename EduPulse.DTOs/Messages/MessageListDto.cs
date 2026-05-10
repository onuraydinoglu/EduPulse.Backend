namespace EduPulse.DTOs.Messages;

public class MessageListDto
{
    public string Id { get; set; } = null!;

    public string SchoolId { get; set; } = null!;

    public string SenderUserId { get; set; } = null!;

    public string SenderFullName { get; set; } = null!;

    public string SenderRoleName { get; set; } = null!;

    public string ReceiverUserId { get; set; } = null!;

    public string ReceiverFullName { get; set; } = null!;

    public string ReceiverRoleName { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public bool IsRead { get; set; }

    public DateTime CreatedDate { get; set; }
}