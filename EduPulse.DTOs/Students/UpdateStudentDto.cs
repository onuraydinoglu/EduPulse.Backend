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

    public string? MotherFullName { get; set; }

    public string? FatherFullName { get; set; }

    public string? MotherPhoneNumber { get; set; }

    public string? FatherPhoneNumber { get; set; }

    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;
}