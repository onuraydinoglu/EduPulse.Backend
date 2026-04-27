namespace EduPulse.DTOs.Auth;

public class RegisterSchoolDto
{
    public string SchoolName { get; set; } = null!;
    public string City { get; set; } = null!;
    public string District { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string SchoolPhoneNumber { get; set; } = null!;
    public string SchoolEmail { get; set; } = null!;

    public string AdminFullName { get; set; } = null!;
    public string AdminEmail { get; set; } = null!;
    public string AdminPassword { get; set; } = null!;
}