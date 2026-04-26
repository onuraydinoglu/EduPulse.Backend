namespace EduPulse.DTOs.Students;

public class CreateStudentDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public string SchoolNumber { get; set; } = null!;
    public string StudentPhone { get; set; } = null!;

    public string SchoolId { get; set; } = null!;
    public string ClassroomId { get; set; } = null!;

    public List<string> ClubIds { get; set; } = new();
    public List<string> ParentIds { get; set; } = new();
}