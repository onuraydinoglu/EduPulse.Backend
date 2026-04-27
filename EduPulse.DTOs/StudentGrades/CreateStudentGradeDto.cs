namespace EduPulse.DTOs.StudentGrades;

public class CreateStudentGradeDto
{
    public string SchoolId { get; set; } = null!;
    public string TeacherId { get; set; } = null!;

    public string StudentId { get; set; } = null!;
    public string LessonId { get; set; } = null!;

    public double Exam1 { get; set; }
    public double Exam2 { get; set; }
    public double Project { get; set; }

    public double Activity1 { get; set; }
    public double Activity2 { get; set; }
    public double Activity3 { get; set; }
}