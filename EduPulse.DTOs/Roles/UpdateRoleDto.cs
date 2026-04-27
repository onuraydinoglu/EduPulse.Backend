namespace EduPulse.DTOs.Roles;

public class UpdateRoleDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
}