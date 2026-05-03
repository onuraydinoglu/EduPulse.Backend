namespace EduPulse.DTOs.Events;

public class EventTeacherDto
{
    public string TeacherId { get; set; } = null!;
    public string? UserId { get; set; }
    public string? FullName { get; set; }
}