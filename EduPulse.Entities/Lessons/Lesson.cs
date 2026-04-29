using EduPulse.Entities.Common;

namespace EduPulse.Entities.Lessons;

public class Lesson : BaseEntity
{
    public string SchoolId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}