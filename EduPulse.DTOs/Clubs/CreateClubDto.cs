namespace EduPulse.DTOs.Clubs;

public class CreateClubDto
{
    public string Name { get; set; } = null!;
    public string? AdvisorTeacherId { get; set; }
}