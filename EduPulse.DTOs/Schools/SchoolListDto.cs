namespace EduPulse.DTOs.Schools;

public class SchoolListDto
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string City { get; set; } = null!;
    public string District { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;
    public string? Email { get; set; }

    public string? PrincipalName { get; set; }

    public bool IsActive { get; set; }
}