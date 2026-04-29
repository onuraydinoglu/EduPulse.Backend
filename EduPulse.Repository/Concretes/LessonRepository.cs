using EduPulse.Entities.Lessons;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Context;
using MongoDB.Driver;

namespace EduPulse.Repository.Concretes;

public class LessonRepository : ILessonRepository
{
    private readonly IMongoCollection<Lesson> _lessons;

    public LessonRepository(MongoDbContext context)
    {
        _lessons = context.Lessons;
    }

    public async Task<List<Lesson>> GetAllAsync()
    {
        return await _lessons
            .Find(x => x.IsActive)
            .ToListAsync();
    }

    public async Task<List<Lesson>> GetBySchoolIdAsync(string schoolId)
    {
        return await _lessons
            .Find(x => x.SchoolId == schoolId && x.IsActive)
            .ToListAsync();
    }

    public async Task<Lesson?> GetByIdAsync(string id)
    {
        return await _lessons
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Lesson?> GetBySchoolIdAndNormalizedNameAsync(string schoolId, string normalizedName)
    {
        return await _lessons
            .Find(x =>
                x.SchoolId == schoolId &&
                x.NormalizedName == normalizedName &&
                x.IsActive)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Lesson lesson)
    {
        await _lessons.InsertOneAsync(lesson);
    }

    public async Task UpdateAsync(Lesson lesson)
    {
        lesson.UpdatedDate = DateTime.UtcNow;

        await _lessons.ReplaceOneAsync(
            x => x.Id == lesson.Id,
            lesson
        );
    }

    public async Task DeleteAsync(string id)
    {
        var update = Builders<Lesson>.Update
            .Set(x => x.IsActive, false)
            .Set(x => x.UpdatedDate, DateTime.UtcNow);

        await _lessons.UpdateOneAsync(x => x.Id == id, update);
    }
}