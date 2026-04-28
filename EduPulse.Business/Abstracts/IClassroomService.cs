using EduPulse.DTOs.Classrooms;
using EduPulse.DTOs.Common;

namespace EduPulse.Business.Abstracts;

public interface IClassroomService
{
    Task<Result<List<ClassroomListDto>>> GetAllForCurrentUserAsync(string? roleName, string? schoolId);
    Task<Result<ClassroomListDto>> GetByIdForCurrentUserAsync(string id, string? roleName, string? schoolId);
    Task<Result> CreateAsync(CreateClassroomDto dto, string? roleName, string? schoolId);
    Task<Result> UpdateAsync(UpdateClassroomDto dto, string? roleName, string? schoolId);
    Task<Result> DeleteAsync(string id, string? roleName, string? schoolId);
}