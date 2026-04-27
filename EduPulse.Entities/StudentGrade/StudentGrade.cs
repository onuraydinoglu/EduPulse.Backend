using EduPulse.Entities.Common;

namespace EduPulse.Entities.StudentGrades;

public class StudentGrade : BaseEntity
{
    public string SchoolId { get; set; } = null!;
    public string TeacherId { get; set; } = null!;
    public string StudentId { get; set; } = null!;
    public string LessonId { get; set; } = null!;

    public int Exam1 { get; set; }
    public int Exam2 { get; set; }

    public int Project { get; set; }

    public int Activity1 { get; set; }
    public int Activity2 { get; set; }
    public int Activity3 { get; set; }

    public double Average { get; set; }

    public bool IsActive { get; set; } = true;
}