namespace EduPulse.DTOs.Auth;

public class LoginResponseDto
{
    public string Id { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string RoleId { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public string? SchoolId { get; set; }

    public string? TeacherId { get; set; }
    public string? StudentId { get; set; }
    public string? ParentId { get; set; }
}