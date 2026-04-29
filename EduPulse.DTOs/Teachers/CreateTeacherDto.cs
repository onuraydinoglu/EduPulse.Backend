namespace EduPulse.DTOs.Teachers;

public class CreateTeacherDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }

    public string? BranchLessonId { get; set; }
    public string? Department { get; set; }
}