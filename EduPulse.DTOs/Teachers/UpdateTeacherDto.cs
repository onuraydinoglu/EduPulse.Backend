namespace EduPulse.DTOs.Teachers;

public class UpdateTeacherDto
{
    public string Id { get; set; } = null!;

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }

    public string? BranchLessonId { get; set; }
    public string? Department { get; set; }

    public bool IsActive { get; set; } = true;
}