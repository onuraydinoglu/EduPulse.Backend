using EduPulse.Entities.StudentGrades;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Context;
using MongoDB.Driver;

namespace EduPulse.Repository.Concretes;

public class StudentGradeRepository : IStudentGradeRepository
{
    private readonly IMongoCollection<StudentGrade> _studentGrades;

    public StudentGradeRepository(MongoDbContext context)
    {
        _studentGrades = context.StudentGrades;
    }

    public async Task<List<StudentGrade>> GetAllAsync()
    {
        return await _studentGrades
            .Find(x => x.IsActive)
            .ToListAsync();
    }

    public async Task<List<StudentGrade>> GetBySchoolIdAsync(string schoolId)
    {
        return await _studentGrades
            .Find(x => x.SchoolId == schoolId && x.IsActive)
            .ToListAsync();
    }

    public async Task<List<StudentGrade>> GetByStudentIdAsync(string studentId)
    {
        return await _studentGrades
            .Find(x => x.StudentId == studentId && x.IsActive)
            .ToListAsync();
    }

    public async Task<List<StudentGrade>> GetByLessonIdAsync(string lessonId)
    {
        return await _studentGrades
            .Find(x => x.LessonId == lessonId && x.IsActive)
            .ToListAsync();
    }

    public async Task<StudentGrade?> GetByIdAsync(string id)
    {
        return await _studentGrades
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<StudentGrade?> GetByStudentAndLessonAsync(string studentId, string lessonId)
    {
        return await _studentGrades
            .Find(x => x.StudentId == studentId && x.LessonId == lessonId && x.IsActive)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(StudentGrade studentGrade)
    {
        await _studentGrades.InsertOneAsync(studentGrade);
    }

    public async Task UpdateAsync(StudentGrade studentGrade)
    {
        studentGrade.UpdatedDate = DateTime.UtcNow;

        await _studentGrades.ReplaceOneAsync(
            x => x.Id == studentGrade.Id,
            studentGrade
        );
    }

    public async Task DeleteAsync(string id)
    {
        var update = Builders<StudentGrade>.Update
            .Set(x => x.IsActive, false)
            .Set(x => x.UpdatedDate, DateTime.UtcNow);

        await _studentGrades.UpdateOneAsync(x => x.Id == id, update);
    }
}