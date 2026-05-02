using EduPulse.Entities.Common;

namespace EduPulse.Entities.Clubs;

public class Club : BaseEntity
{
    public string SchoolId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;
    public string AdvisorTeacherId { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}