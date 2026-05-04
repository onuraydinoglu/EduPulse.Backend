namespace EduPulse.DTOs.EventMembers;

public class EventMemberListDto
{
    public string Id { get; set; } = null!;

    public string EventId { get; set; } = null!;
    public string EventName { get; set; } = "-";

    public string StudentId { get; set; } = null!;
    public string StudentFullName { get; set; } = "-";
    public string StudentNumber { get; set; } = "-";

    public string ClassroomId { get; set; } = "";
    public string ClassroomName { get; set; } = "-";

    public bool IsPaid { get; set; }
    public decimal PaidAmount { get; set; }

    public bool IsActive { get; set; }
}