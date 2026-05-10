namespace EduPulse.DTOs.Messages;

public class CreateMessageDto
{
    public string ReceiverUserId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;
}