using EduPulse.Entities.Common;

namespace EduPulse.Entities.PersonalNotes;

public class PersonalNote : BaseEntity
{
    public string SchoolId { get; set; } = null!;
    public string UserId { get; set; } = null!;

    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;

    public bool IsActive { get; set; } = true;
}