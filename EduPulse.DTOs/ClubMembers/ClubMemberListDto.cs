namespace EduPulse.DTOs.ClubMembers;

public class ClubMemberListDto
{
    public string Id { get; set; } = null!;
    public string ClubId { get; set; } = null!;
    public string StudentId { get; set; } = null!;
    public string? StudentFullName { get; set; }
    public string? StudentNumber { get; set; }
    public bool IsActive { get; set; }
}