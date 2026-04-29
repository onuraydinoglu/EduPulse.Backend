namespace EduPulse.DTOs.Teachers;

public class TeacherListDto
{
    public string Id { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string SchoolId { get; set; } = null!;

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }

    public string? BranchLessonId { get; set; }
    public string? BranchLessonName { get; set; }

    public string? Department { get; set; }

    public bool IsActive { get; set; }
}