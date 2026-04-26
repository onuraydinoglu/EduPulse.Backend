namespace EduPulse.DTOs.Classrooms;

public class ClassroomListDto
{
    public string Id { get; set; } = null!;

    public string SchoolId { get; set; } = null!;

    public int Grade { get; set; }
    public string Section { get; set; } = null!;

    public string? TeacherId { get; set; }

    public bool IsActive { get; set; }
}