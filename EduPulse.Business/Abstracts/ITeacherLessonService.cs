using EduPulse.DTOs.Common;
using EduPulse.DTOs.TeacherLessons;

namespace EduPulse.Business.Abstracts;

public interface ITeacherLessonService
{
    Task<Result<List<TeacherLessonListDto>>> GetAllAsync();
    Task<Result<List<TeacherLessonListDto>>> GetBySchoolIdAsync(string schoolId);
    Task<Result<List<TeacherLessonListDto>>> GetByTeacherIdAsync(string teacherId);
    Task<Result<TeacherLessonListDto>> GetByIdAsync(string id);
    Task<Result> CreateAsync(CreateTeacherLessonDto dto);
    Task<Result> UpdateAsync(UpdateTeacherLessonDto dto);
    Task<Result> DeleteAsync(string id);
}