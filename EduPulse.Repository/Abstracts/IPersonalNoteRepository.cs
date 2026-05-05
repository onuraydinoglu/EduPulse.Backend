using EduPulse.Entities.PersonalNotes;

public interface IPersonalNoteRepository
{
    Task<List<PersonalNote>> GetByUserAsync(string schoolId, string userId);
    Task<PersonalNote?> GetByIdAsync(string id);
    Task<PersonalNote?> GetOwnedNoteAsync(string id, string schoolId, string userId);
    Task CreateAsync(PersonalNote note);
    Task UpdateAsync(PersonalNote note);
    Task DeleteAsync(string id);
}