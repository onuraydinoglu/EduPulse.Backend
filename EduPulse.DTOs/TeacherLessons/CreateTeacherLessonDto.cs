namespace EduPulse.DTOs.TeacherLessons;

public class CreateTeacherLessonDto
{
    public string TeacherId { get; set; } = null!;

    public string LessonId { get; set; } = null!;

    // Geriye dönük uyumluluk için bırakıldı.
    // Eski frontend tek classroomId gönderirse yine çalışır.
    public string? ClassroomId { get; set; }

    // Yeni yapı: birden fazla sınıf seçimi.
    public List<string> ClassroomIds { get; set; } = new();
}