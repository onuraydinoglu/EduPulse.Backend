using EduPulse.Entities.Schools;

public interface ISchoolRepository
{
    Task<List<School>> GetAllAsync();
    Task<School?> GetByIdAsync(string id);
    Task<School?> GetByEmailAsync(string email);
    Task CreateAsync(School school);
    Task UpdateAsync(School school);
    Task DeleteAsync(string id);
}