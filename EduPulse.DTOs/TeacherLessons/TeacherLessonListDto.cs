namespace EduPulse.DTOs.TeacherLessons;

public class TeacherLessonListDto
{
    public string Id { get; set; } = null!;

    public string SchoolId { get; set; } = null!;

    public string TeacherId { get; set; } = null!;
    public string TeacherName { get; set; } = null!;

    public string LessonId { get; set; } = null!;
    public string LessonName { get; set; } = null!;

    public string ClassroomId { get; set; } = null!;
    public string ClassroomName { get; set; } = null!;

    public bool IsActive { get; set; }
}