namespace EduPulse.DTOs.ClubMembers;

public class ClubMemberListDto
{
    public string Id { get; set; } = null!;

    public string ClubId { get; set; } = null!;
    public string ClubName { get; set; } = null!;

    public string StudentId { get; set; } = null!;
    public string StudentFullName { get; set; } = null!;
    public string StudentNumber { get; set; } = null!;

    public string ClassroomId { get; set; } = null!;
    public string ClassroomName { get; set; } = null!;

    public bool IsActive { get; set; }
}