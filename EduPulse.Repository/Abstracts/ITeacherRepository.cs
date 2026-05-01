using EduPulse.Entities.Teachers;

namespace EduPulse.Repository.Abstracts;

public interface ITeacherRepository
{
    Task<List<Teacher>> GetAllAsync();
    Task<List<Teacher>> GetBySchoolIdAsync(string schoolId);
    Task<List<Teacher>> GetActiveBySchoolIdAsync(string schoolId);

    Task<Teacher?> GetByIdAsync(string id);
    Task<Teacher?> GetByUserIdAsync(string userId);

    Task CreateAsync(Teacher teacher);
    Task UpdateAsync(Teacher teacher);
    Task DeleteAsync(string id);
}