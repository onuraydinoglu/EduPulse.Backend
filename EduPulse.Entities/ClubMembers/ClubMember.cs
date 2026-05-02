using EduPulse.Entities.Common;

namespace EduPulse.Entities.ClubMembers;

public class ClubMember : BaseEntity
{
    public string SchoolId { get; set; } = null!;
    public string ClubId { get; set; } = null!;
    public string StudentId { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}