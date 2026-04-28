namespace EduPulse.DTOs.Classrooms;

public class CreateClassroomDto
{
    public int Grade { get; set; }
    public string Section { get; set; } = null!;
    public string? TeacherId { get; set; }
}