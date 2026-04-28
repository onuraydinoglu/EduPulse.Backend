using EduPulse.DTOs.Common;
using EduPulse.DTOs.Users;

namespace EduPulse.Business.Abstracts;

public interface IUserService
{
    Task<Result<List<UserListDto>>> GetAllAsync();
    Task<Result<UserListDto>> GetByIdAsync(string id);
    Task<Result<List<UserListDto>>> GetBySchoolIdAsync(string schoolId);

    Task<Result<List<UserListDto>>> GetTeachersAsync(string? schoolId);
    Task<Result<List<UserListDto>>> GetOfficersAsync(string? schoolId);

    Task<Result<List<UserListDto>>> GetAllForCurrentUserAsync(string? currentRoleName, string? currentSchoolId);
    Task<Result<UserListDto>> GetByIdForCurrentUserAsync(string id, string? currentRoleName, string? currentSchoolId);

    Task<Result> CreateUserAsync(CreateUserDto dto, string? schoolId, string roleName);
    Task<Result> UpdateForCurrentUserAsync(UpdateUserDto dto, string? currentRoleName, string? currentSchoolId);
    Task<Result> DeleteForCurrentUserAsync(string id, string? currentRoleName, string? currentSchoolId);

    Task<Result> UpdateAsync(UpdateUserDto dto);
    Task<Result> DeleteAsync(string id);
}