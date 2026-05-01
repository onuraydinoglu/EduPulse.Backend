using EduPulse.Entities.Students;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Context;
using MongoDB.Driver;

namespace EduPulse.Repository.Concretes;

public class StudentRepository : IStudentRepository
{
    private readonly IMongoCollection<Student> _students;

    public StudentRepository(MongoDbContext context)
    {
        _students = context.Students;
    }

    public async Task<List<Student>> GetAllAsync()
    {
        return await _students.Find(_ => true).ToListAsync();
    }

    public async Task<List<Student>> GetBySchoolIdAsync(string schoolId)
    {
        return await _students
            .Find(x => x.SchoolId == schoolId)
            .ToListAsync();
    }

    public async Task<List<Student>> GetByClassroomIdAsync(string classroomId)
    {
        return await _students
            .Find(x => x.ClassroomId == classroomId)
            .ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(string id)
    {
        return await _students.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Student?> GetByUserIdAsync(string userId)
    {
        return await _students.Find(x => x.UserId == userId).FirstOrDefaultAsync();
    }

    public async Task<Student?> GetBySchoolIdAndStudentNumberAsync(
        string schoolId,
        string studentNumber
    )
    {
        return await _students
            .Find(x => x.SchoolId == schoolId && x.StudentNumber == studentNumber)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Student student)
    {
        await _students.InsertOneAsync(student);
    }

    public async Task UpdateAsync(Student student)
    {
        student.UpdatedDate = DateTime.UtcNow;
        await _students.ReplaceOneAsync(x => x.Id == student.Id, student);
    }

    public async Task DeleteAsync(string id)
    {
        var update = Builders<Student>.Update
            .Set(x => x.IsActive, false)
            .Set(x => x.UpdatedDate, DateTime.UtcNow);

        await _students.UpdateOneAsync(x => x.Id == id, update);
    }
}