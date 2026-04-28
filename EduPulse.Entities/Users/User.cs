using EduPulse.Entities.Common;

namespace EduPulse.Entities.Users;

public class User : BaseEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;

    public string RoleId { get; set; } = null!;
    public string RoleName { get; set; } = null!;

    public string? SchoolId { get; set; }

    public bool IsActive { get; set; } = true;
}