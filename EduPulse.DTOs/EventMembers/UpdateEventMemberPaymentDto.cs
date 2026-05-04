namespace EduPulse.DTOs.EventMembers;

public class UpdateEventMemberPaymentDto
{
    public string Id { get; set; } = null!;

    public bool IsPaid { get; set; }
    public decimal PaidAmount { get; set; }
}