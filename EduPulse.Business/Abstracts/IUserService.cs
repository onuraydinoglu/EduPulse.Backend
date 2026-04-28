using EduPulse.DTOs.Common;
using EduPulse.DTOs.Users;

namespace EduPulse.Business.Abstracts;

public interface IUserService
{
    Task<Result<List<UserListDto>>> GetAllAsync();
    Task<Result<UserListDto>> GetByIdAsync(string id);
    Task<Result<List<UserListDto>>> GetBySchoolIdAsync(string schoolId);

    Task<Result> CreateUserAsync(CreateUserDto dto, string? schoolId);
    Task<Result> UpdateAsync(UpdateUserDto dto);
    Task<Result> DeleteAsync(string id);
}