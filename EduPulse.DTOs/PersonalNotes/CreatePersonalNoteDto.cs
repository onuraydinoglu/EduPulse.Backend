namespace EduPulse.DTOs.PersonalNotes;

public class CreatePersonalNoteDto
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public bool IsPinned { get; set; } = false;
}