using EduPulse.DTOs.Common;
using EduPulse.DTOs.TeacherLessons;

namespace EduPulse.Business.Abstracts;

public interface ITeacherLessonService
{
    Task<Result<List<TeacherLessonListDto>>> GetAllForCurrentUserAsync(
        string? roleName,
        string? schoolId,
        string? currentUserId);

    Task<Result<TeacherLessonListDto>> GetByIdForCurrentUserAsync(
        string id,
        string? roleName,
        string? schoolId,
        string? currentUserId);

    Task<Result<string>> CreateAsync(CreateTeacherLessonDto dto, string? schoolId);

    Task<Result<string>> UpdateAsync(UpdateTeacherLessonDto dto, string? schoolId);

    Task<Result<string>> DeleteAsync(string id);
}