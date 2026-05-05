namespace EduPulse.DTOs.PersonalNotes;

public class UpdatePersonalNoteDto
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public bool IsActive { get; set; }
}