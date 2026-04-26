using EduPulse.Entities.Classrooms;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Context;
using MongoDB.Driver;

namespace EduPulse.Repository.Concretes;

public class ClassroomRepository : IClassroomRepository
{
    private readonly IMongoCollection<Classroom> _classrooms;

    public ClassroomRepository(MongoDbContext context)
    {
        _classrooms = context.Classrooms;
    }

    public async Task<List<Classroom>> GetAllAsync()
    {
        return await _classrooms.Find(x => x.IsActive).ToListAsync();
    }

    public async Task<List<Classroom>> GetBySchoolIdAsync(string schoolId)
    {
        return await _classrooms
            .Find(x => x.SchoolId == schoolId && x.IsActive)
            .ToListAsync();
    }

    public async Task<Classroom?> GetByIdAsync(string id)
    {
        return await _classrooms.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Classroom classroom)
    {
        await _classrooms.InsertOneAsync(classroom);
    }

    public async Task UpdateAsync(Classroom classroom)
    {
        classroom.UpdatedDate = DateTime.UtcNow;

        await _classrooms.ReplaceOneAsync(
            x => x.Id == classroom.Id,
            classroom
        );
    }

    public async Task DeleteAsync(string id)
    {
        var update = Builders<Classroom>.Update
            .Set(x => x.IsActive, false)
            .Set(x => x.UpdatedDate, DateTime.UtcNow);

        await _classrooms.UpdateOneAsync(x => x.Id == id, update);
    }
}