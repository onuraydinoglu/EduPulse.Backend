using EduPulse.Entities.Common;

namespace EduPulse.Entities.Classrooms;

public class Classroom : BaseEntity
{
    public string SchoolId { get; set; } = null!;
    public int Grade { get; set; }
    public string Section { get; set; } = null!;
    public string? TeacherId { get; set; }
    public bool IsActive { get; set; } = true;
}