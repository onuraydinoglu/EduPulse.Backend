using EduPulse.Entities.Schools;

namespace EduPulse.Repository.Abstracts;

public interface ISchoolRepository
{
    Task<List<School>> GetAllAsync();
    Task<School?> GetByIdAsync(string id);
    Task<School?> GetBySchoolCodeAsync(string schoolCode);

    Task<bool> SchoolCodeExistsAsync(string schoolCode);
    Task<bool> NameExistsAsync(string name);
    Task<bool> NameExistsForUpdateAsync(string id, string name);

    Task CreateAsync(School school);
    Task UpdateAsync(School school);
    Task DeleteAsync(string id);
}