namespace EduPulse.DTOs.Students;

public class StudentDetailDto
{
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public string SchoolId { get; set; } = null!;
    public string ClassroomId { get; set; } = null!;

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string FullName => $"{FirstName} {LastName}";

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string StudentNumber { get; set; } = null!;

    public string? ClassroomName { get; set; }

    public bool IsActive { get; set; }
}