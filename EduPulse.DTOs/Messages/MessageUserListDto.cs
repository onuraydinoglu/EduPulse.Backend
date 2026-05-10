namespace EduPulse.DTOs.Messages;

public class MessageUserListDto
{
    public string UserId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public string Email { get; set; } = null!;
}