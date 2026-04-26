using EduPulse.Entities.Schools;

namespace EduPulse.Repository.Abstracts;

public interface ISchoolRepository
{
    Task<List<School>> GetAllAsync();
    Task<School?> GetByIdAsync(string id);
    Task CreateAsync(School school);
    Task UpdateAsync(School school);
    Task DeleteAsync(string id);
}