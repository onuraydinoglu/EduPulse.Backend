namespace EduPulse.DTOs.Clubs;

public class UpdateClubDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string AdvisorTeacherId { get; set; } = null!;
    public bool IsActive { get; set; }
}