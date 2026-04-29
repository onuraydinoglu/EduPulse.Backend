namespace EduPulse.DTOs.Lessons;

public class UpdateLessonDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
}