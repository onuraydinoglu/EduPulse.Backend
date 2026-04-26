namespace EduPulse.DTOs.Teachers;

public class TeacherListDto
{
    public string Id { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;
    public string? Email { get; set; }

    public string SchoolId { get; set; } = null!;

    public bool IsActive { get; set; }
}