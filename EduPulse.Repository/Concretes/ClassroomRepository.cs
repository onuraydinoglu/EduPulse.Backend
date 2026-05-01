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
        return await _classrooms
            .Find(x => true)
            .SortBy(x => x.Grade)
            .ThenBy(x => x.Section)
            .ToListAsync();
    }

    public async Task<List<Classroom>> GetBySchoolIdAsync(string schoolId)
    {
        return await _classrooms
            .Find(x => x.SchoolId == schoolId)
            .SortBy(x => x.Grade)
            .ThenBy(x => x.Section)
            .ToListAsync();
    }

    public async Task<Classroom?> GetByIdAsync(string id)
    {
        return await _classrooms
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Classroom?> GetBySchoolIdAndTeacherIdAsync(string schoolId, string teacherId)
    {
        return await _classrooms
            .Find(x =>
                x.SchoolId == schoolId &&
                x.TeacherId == teacherId)
            .FirstOrDefaultAsync();
    }

    public async Task<Classroom?> GetBySchoolIdAndTeacherIdExceptClassroomIdAsync(
        string schoolId,
        string teacherId,
        string classroomId)
    {
        return await _classrooms
            .Find(x =>
                x.SchoolId == schoolId &&
                x.TeacherId == teacherId &&
                x.Id != classroomId)
            .FirstOrDefaultAsync();
    }

    public async Task<Classroom?> GetBySchoolGradeSectionAsync(string schoolId, int grade, string section)
    {
        return await _classrooms
            .Find(x =>
                x.SchoolId == schoolId &&
                x.Grade == grade &&
                x.Section.ToLower() == section.ToLower())
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Classroom classroom)
    {
        classroom.CreatedDate = DateTime.UtcNow;
        classroom.UpdatedDate = null;

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
        await _classrooms.DeleteOneAsync(x => x.Id == id);
    }
}