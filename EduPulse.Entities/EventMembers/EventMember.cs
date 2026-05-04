using EduPulse.Entities.Common;

namespace EduPulse.Entities.EventMembers;

public class EventMember : BaseEntity
{
    public string SchoolId { get; set; } = null!;
    public string EventId { get; set; } = null!;
    public string StudentId { get; set; } = null!;

    public bool IsPaid { get; set; } = false;
    public decimal PaidAmount { get; set; } = 0;

    public bool IsActive { get; set; } = true;
}