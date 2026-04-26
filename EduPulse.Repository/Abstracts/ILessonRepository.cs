using EduPulse.Entities.Lessons;

namespace EduPulse.Repository.Abstracts;

public interface ILessonRepository
{
    Task<List<Lesson>> GetAllAsync();
    Task<List<Lesson>> GetBySchoolIdAsync(string schoolId);
    Task<Lesson?> GetByIdAsync(string id);
    Task CreateAsync(Lesson lesson);
    Task UpdateAsync(Lesson lesson);
    Task DeleteAsync(string id);
}