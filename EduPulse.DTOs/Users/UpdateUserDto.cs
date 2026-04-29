namespace EduPulse.DTOs.Users;

public class UpdateUserDto
{
    public string Id { get; set; } = null!;

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;

    public bool IsActive { get; set; }
}