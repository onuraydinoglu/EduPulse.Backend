namespace EduPulse.DTOs.EventMembers;

public class CreateEventMemberDto
{
    public string EventId { get; set; } = null!;
    public string StudentId { get; set; } = null!;

    public bool IsPaid { get; set; } = false;
    public decimal PaidAmount { get; set; } = 0;
}