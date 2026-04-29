using EduPulse.DTOs.Common;
using EduPulse.DTOs.Lessons;

namespace EduPulse.Business.Abstracts;

public interface ILessonService
{
    Task<Result<List<LessonListDto>>> GetAllForCurrentUserAsync(string? roleName, string? schoolId);
    Task<Result<LessonListDto>> GetByIdForCurrentUserAsync(string id, string? roleName, string? schoolId);
    Task<Result> CreateAsync(CreateLessonDto dto, string? roleName, string? schoolId);
    Task<Result> UpdateAsync(UpdateLessonDto dto, string? roleName, string? schoolId);
    Task<Result> DeleteAsync(string id, string? roleName, string? schoolId);
}