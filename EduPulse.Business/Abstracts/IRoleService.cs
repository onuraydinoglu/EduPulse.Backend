using EduPulse.DTOs.Common;
using EduPulse.DTOs.Roles;

namespace EduPulse.Business.Abstracts;

public interface IRoleService
{
    Task<Result<List<RoleListDto>>> GetAllAsync();
    Task<Result<RoleListDto>> GetByIdAsync(string id);
    Task<Result> CreateAsync(CreateRoleDto dto);
    Task<Result> UpdateAsync(UpdateRoleDto dto);
    Task<Result> DeleteAsync(string id);
}