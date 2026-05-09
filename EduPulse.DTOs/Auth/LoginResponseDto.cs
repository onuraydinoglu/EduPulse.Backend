namespace EduPulse.DTOs.Auth;

public class LoginResponseDto
{
    public string Id { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string FullName => $"{FirstName} {LastName}";

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public string? SchoolId { get; set; }

    public string? TeacherId { get; set; }

    public string Token { get; set; } = null!;
}