namespace EduPulse.DTOs.Students;

public class UpdateStudentDto
{
    public string Id { get; set; } = null!;

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string ClassroomId { get; set; } = null!;
    public string StudentNumber { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}