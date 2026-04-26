using EduPulse.Entities.Common;

namespace EduPulse.Entities.TeacherLessons
{
    public class TeacherLesson : BaseEntity
    {
        public string SchoolId { get; set; } = null!;

        public string TeacherId { get; set; } = null!;
        public string LessonId { get; set; } = null!;
        public string ClassroomId { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}
