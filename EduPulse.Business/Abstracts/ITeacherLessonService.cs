using EduPulse.DTOs.Common;
using EduPulse.DTOs.TeacherLessons;

namespace EduPulse.Business.Abstracts;

public interface ITeacherLessonService
{
    Task<Result<List<TeacherLessonListDto>>> GetAllForCurrentUserAsync(string? roleName, string? schoolId);
    Task<Result<TeacherLessonListDto>> GetByIdForCurrentUserAsync(string id, string? roleName, string? schoolId);

    Task<Result> CreateAsync(CreateTeacherLessonDto dto, string? schoolId);
    Task<Result> UpdateAsync(UpdateTeacherLessonDto dto, string? schoolId);
    Task<Result> DeleteAsync(string id);
}