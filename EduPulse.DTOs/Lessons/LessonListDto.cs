namespace EduPulse.DTOs.Lessons;

public class LessonListDto
{
    public string Id { get; set; } = null!;
    public string SchoolId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
}