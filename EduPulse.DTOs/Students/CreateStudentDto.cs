namespace EduPulse.DTOs.Students;

public class CreateStudentDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string ClassroomId { get; set; } = null!;
    public string StudentNumber { get; set; } = null!;
}