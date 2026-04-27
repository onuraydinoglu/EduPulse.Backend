using EduPulse.Entities.Roles;

namespace EduPulse.Repository.Abstracts;

public interface IRoleRepository
{
    Task<List<Role>> GetAllAsync();
    Task<Role?> GetByIdAsync(string id);
    Task<Role?> GetByNameAsync(string name);
    Task CreateAsync(Role role);
    Task UpdateAsync(Role role);
    Task DeleteAsync(string id);
}