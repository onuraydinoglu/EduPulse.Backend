using EduPulse.Entities.Students;

namespace EduPulse.Repository.Abstracts;

public interface IStudentRepository
{
    Task<List<Student>> GetAllAsync();
    Task<List<Student>> GetBySchoolIdAsync(string schoolId);
    Task<List<Student>> GetByClassroomIdAsync(string classroomId);
    Task<Student?> GetByIdAsync(string id);
    Task CreateAsync(Student student);
    Task UpdateAsync(Student student);
    Task DeleteAsync(string id);
}