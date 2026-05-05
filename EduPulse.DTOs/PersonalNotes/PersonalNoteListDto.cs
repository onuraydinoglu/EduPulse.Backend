namespace EduPulse.DTOs.PersonalNotes;

public class PersonalNoteListDto
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public bool IsPinned { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}