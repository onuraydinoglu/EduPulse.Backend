namespace EduPulse.DTOs.Clubs;

public class ClubListDto
{
    public string Id { get; set; } = null!;
    public string SchoolId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string AdvisorTeacherId { get; set; } = null!;
    public string? AdvisorTeacherFullName { get; set; }
    public int MemberCount { get; set; }
    public bool IsActive { get; set; }
}