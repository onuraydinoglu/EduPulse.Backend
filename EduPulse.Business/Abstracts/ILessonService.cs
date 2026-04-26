using EduPulse.DTOs.Lessons;

namespace EduPulse.Business.Abstracts;

public interface ILessonService
{
    Task<List<LessonListDto>> GetAllAsync();
    Task<List<LessonListDto>> GetBySchoolIdAsync(string schoolId);
    Task<LessonListDto?> GetByIdAsync(string id);
    Task CreateAsync(CreateLessonDto dto);
    Task UpdateAsync(UpdateLessonDto dto);
    Task DeleteAsync(string id);
}