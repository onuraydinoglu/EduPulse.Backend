using EduPulse.Entities.Common;

namespace EduPulse.Entities.Teachers;

public class Teacher : BaseEntity
{
    public string UserId { get; set; } = null!;
    public string SchoolId { get; set; } = null!;

    public string? BranchLessonId { get; set; }
    public string? Department { get; set; }

    public bool IsActive { get; set; } = true;
}