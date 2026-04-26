namespace EduPulse.DTOs.Parents;

public class CreateParentDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;
    public string? Email { get; set; }

    public string SchoolId { get; set; } = null!;
}