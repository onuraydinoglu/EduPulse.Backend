namespace EduPulse.DTOs.Parents;

public class ParentListDto
{
    public string Id { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;
    public string? Email { get; set; }

    public string SchoolId { get; set; } = null!;

    public bool IsActive { get; set; }
}