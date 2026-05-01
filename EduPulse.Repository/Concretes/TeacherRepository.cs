using EduPulse.Entities.Teachers;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Context;
using MongoDB.Driver;

namespace EduPulse.Repository.Concretes;

public class TeacherRepository : ITeacherRepository
{
    private readonly IMongoCollection<Teacher> _teachers;

    public TeacherRepository(MongoDbContext context)
    {
        _teachers = context.Teachers;
    }

    public async Task<List<Teacher>> GetAllAsync()
    {
        return await _teachers.Find(_ => true).ToListAsync();
    }

    public async Task<List<Teacher>> GetBySchoolIdAsync(string schoolId)
    {
        return await _teachers
            .Find(x => x.SchoolId == schoolId)
            .ToListAsync();
    }

    public async Task<Teacher?> GetByIdAsync(string id)
    {
        return await _teachers
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Teacher?> GetByUserIdAsync(string userId)
    {
        return await _teachers
            .Find(x => x.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Teacher teacher)
    {
        await _teachers.InsertOneAsync(teacher);
    }

    public async Task UpdateAsync(Teacher teacher)
    {
        teacher.UpdatedDate = DateTime.UtcNow;

        await _teachers.ReplaceOneAsync(
            x => x.Id == teacher.Id,
            teacher
        );
    }

    public async Task DeleteAsync(string id)
    {
        var update = Builders<Teacher>.Update
            .Set(x => x.IsActive, false)
            .Set(x => x.UpdatedDate, DateTime.UtcNow);

        await _teachers.UpdateOneAsync(x => x.Id == id, update);
    }
}