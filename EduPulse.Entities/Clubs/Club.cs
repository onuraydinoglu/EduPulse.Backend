using EduPulse.Entities.Common;

public class Club : BaseEntity
{
    public string Name { get; set; } = null!; // Robotik, Satranç
    public string SchoolId { get; set; } = null!;

    public string? AdvisorTeacherId { get; set; } // Kulüp danışman hocası

    public bool IsActive { get; set; } = true;
}
