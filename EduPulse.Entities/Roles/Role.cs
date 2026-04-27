using EduPulse.Entities.Common;

namespace EduPulse.Entities.Roles;

public class Role : BaseEntity
{
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}