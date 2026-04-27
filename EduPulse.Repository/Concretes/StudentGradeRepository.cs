using EduPulse.Entities.StudentGrades;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Context;
using MongoDB.Driver;

namespace EduPulse.Repository.Concretes;

public class StudentGradeRepository : IStudentGradeRepository
{
    private readonly IMongoCollection<StudentGrade> _collection;

    public StudentGradeRepository(MongoDbContext context)
    {
        _collection = context.StudentGrades;
    }

    public async Task<List<StudentGrade>> GetAllAsync()
    {
        return await _collection.Find(x => true).ToListAsync();
    }

    public async Task<List<StudentGrade>> GetBySchoolIdAsync(string schoolId)
    {
        return await _collection
            .Find(x => x.SchoolId == schoolId)
            .ToListAsync();
    }

    public async Task<List<StudentGrade>> GetByTeacherIdAsync(string teacherId)
    {
        return await _collection
            .Find(x => x.TeacherId == teacherId)
            .ToListAsync();
    }

    public async Task<List<StudentGrade>> GetByStudentIdAsync(string studentId)
    {
        return await _collection
            .Find(x => x.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<List<StudentGrade>> GetByLessonIdAsync(string lessonId)
    {
        return await _collection
            .Find(x => x.LessonId == lessonId)
            .ToListAsync();
    }

    public async Task<StudentGrade?> GetByIdAsync(string id)
    {
        return await _collection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<StudentGrade?> GetByStudentAndLessonAsync(string studentId, string lessonId)
    {
        return await _collection
            .Find(x => x.StudentId == studentId && x.LessonId == lessonId)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(StudentGrade studentGrade)
    {
        await _collection.InsertOneAsync(studentGrade);
    }

    public async Task UpdateAsync(StudentGrade studentGrade)
    {
        await _collection.ReplaceOneAsync(x => x.Id == studentGrade.Id, studentGrade);
    }

    public async Task DeleteAsync(string id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }
}