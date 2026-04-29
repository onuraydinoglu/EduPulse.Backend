using EduPulse.DTOs.Common;
using EduPulse.DTOs.Students;

namespace EduPulse.Business.Abstracts;

public interface IStudentService
{
    Task<Result<List<StudentListDto>>> GetAllForCurrentUserAsync(string? currentRoleName, string? currentSchoolId);
    Task<Result<StudentListDto>> GetByIdForCurrentUserAsync(string id, string? currentRoleName, string? currentSchoolId);

    Task<Result> CreateForCurrentUserAsync(CreateStudentDto dto, string? currentRoleName, string? currentSchoolId);
    Task<Result> UpdateForCurrentUserAsync(UpdateStudentDto dto, string? currentRoleName, string? currentSchoolId);
    Task<Result> DeleteForCurrentUserAsync(string id, string? currentRoleName, string? currentSchoolId);
}