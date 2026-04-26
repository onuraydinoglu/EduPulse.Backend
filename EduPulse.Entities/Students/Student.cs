using EduPulse.Entities.Common;

namespace EduPulse.Entities.Students;

public class Student : BaseEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string SchoolNumber { get; set; } = null!;
    public string StudentPhone { get; set; } = null!;

    public string SchoolId { get; set; } = null!;
    public string ClassrommId { get; set; } = null!;
    public string? ClubId { get; set; }

    public List<string> ParentIds { get; set; } = new();

    public bool IsActive { get; set; } = true;
}