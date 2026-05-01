using EduPulse.DTOs.Common;
using EduPulse.DTOs.Teachers;

namespace EduPulse.Business.Abstracts;

public interface ITeacherService
{
    Task<Result<List<TeacherListDto>>> GetAllForCurrentUserAsync(
        string? currentRoleName,
        string? currentSchoolId);

    Task<Result<List<TeacherListDto>>> GetActiveBySchoolIdAsync(string schoolId);

    Task<Result<TeacherListDto>> GetByIdForCurrentUserAsync(
        string id,
        string? currentRoleName,
        string? currentSchoolId);

    Task<Result> CreateForCurrentUserAsync(
        CreateTeacherDto dto,
        string? currentRoleName,
        string? currentSchoolId);

    Task<Result> UpdateForCurrentUserAsync(
        UpdateTeacherDto dto,
        string? currentRoleName,
        string? currentSchoolId);

    Task<Result> DeleteForCurrentUserAsync(
        string id,
        string? currentRoleName,
        string? currentSchoolId);
}