namespace EduPulse.DTOs.Events;

public class CreateEventDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Location { get; set; } = null!;

    public DateTime EventDate { get; set; }
    public string StartTime { get; set; } = null!;
    public string? EndTime { get; set; }

    public bool IsPaid { get; set; }
    public decimal PricePerStudent { get; set; }

    public int? Quota { get; set; }

    public List<string> ResponsibleTeacherIds { get; set; } = new();
}