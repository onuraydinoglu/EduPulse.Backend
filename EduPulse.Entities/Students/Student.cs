using EduPulse.Entities.Common;

namespace EduPulse.Entities.Students;

public class Student : BaseEntity
{
    public string UserId { get; set; } = null!;
    public string SchoolId { get; set; } = null!;
    public string ClassroomId { get; set; } = null!;
    public string StudentNumber { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}