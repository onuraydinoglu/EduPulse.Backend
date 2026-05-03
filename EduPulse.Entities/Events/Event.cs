using EduPulse.Entities.Common;

namespace EduPulse.Entities.Events;

public class Event : BaseEntity
{
    public string SchoolId { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;

    public string? Description { get; set; }
    public string Location { get; set; } = null!;

    public DateTime EventDate { get; set; }
    public string StartTime { get; set; } = null!;
    public string? EndTime { get; set; }

    public bool IsPaid { get; set; }
    public decimal PricePerStudent { get; set; }

    public int? Quota { get; set; }

    public List<string> ResponsibleTeacherIds { get; set; } = new();

    public bool IsActive { get; set; } = true;
}