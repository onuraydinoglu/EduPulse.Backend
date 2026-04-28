using EduPulse.Entities.Users;

namespace EduPulse.Repository.Abstracts;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetBySchoolIdAsync(string schoolId);

    Task<List<User>> GetByRoleNameAsync(string roleName);
    Task<List<User>> GetBySchoolIdAndRoleNameAsync(string schoolId, string roleName);

    Task<bool> ExistsSchoolAdminAsync(string schoolId);

    Task CreateAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(string id);
}