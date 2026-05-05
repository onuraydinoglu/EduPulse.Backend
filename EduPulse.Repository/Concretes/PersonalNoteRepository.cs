using EduPulse.Entities.PersonalNotes;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Context;
using MongoDB.Driver;

namespace EduPulse.Repository.Concretes;

public class PersonalNoteRepository : IPersonalNoteRepository
{
    private readonly IMongoCollection<PersonalNote> _personalNotes;

    public PersonalNoteRepository(MongoDbContext context)
    {
        _personalNotes = context.PersonalNotes;
    }

    public async Task<List<PersonalNote>> GetByUserAsync(string schoolId, string userId)
    {
        return await _personalNotes
            .Find(x =>
                x.SchoolId == schoolId &&
                x.UserId == userId)
            .SortByDescending(x => x.IsPinned)
            .ThenByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<PersonalNote?> GetByIdAsync(string id)
    {
        return await _personalNotes
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<PersonalNote?> GetOwnedNoteAsync(
        string id,
        string schoolId,
        string userId)
    {
        return await _personalNotes
            .Find(x =>
                x.Id == id &&
                x.SchoolId == schoolId &&
                x.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(PersonalNote personalNote)
    {
        await _personalNotes.InsertOneAsync(personalNote);
    }

    public async Task UpdateAsync(PersonalNote personalNote)
    {
        await _personalNotes.ReplaceOneAsync(
            x => x.Id == personalNote.Id,
            personalNote);
    }

    public async Task DeleteAsync(string id)
    {
        await _personalNotes.DeleteOneAsync(x => x.Id == id);
    }
}