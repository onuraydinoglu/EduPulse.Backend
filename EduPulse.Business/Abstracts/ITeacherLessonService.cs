using EduPulse.DTOs.TeacherLessons;

namespace EduPulse.Business.Abstracts;

public interface ITeacherLessonService
{
    Task<List<TeacherLessonListDto>> GetAllAsync();
    Task<List<TeacherLessonListDto>> GetBySchoolIdAsync(string schoolId);
    Task<List<TeacherLessonListDto>> GetByTeacherIdAsync(string teacherId);
    Task<TeacherLessonListDto?> GetByIdAsync(string id);
    Task CreateAsync(CreateTeacherLessonDto dto);
    Task UpdateAsync(UpdateTeacherLessonDto dto);
    Task DeleteAsync(string id);
}