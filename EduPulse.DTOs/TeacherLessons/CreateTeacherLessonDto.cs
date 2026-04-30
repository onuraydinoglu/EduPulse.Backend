namespace EduPulse.DTOs.TeacherLessons;

public class CreateTeacherLessonDto
{
    public string TeacherId { get; set; } = null!;
    public string LessonId { get; set; } = null!;
    public string ClassroomId { get; set; } = null!;
}