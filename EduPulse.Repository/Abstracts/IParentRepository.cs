using EduPulse.Entities.Parents;

namespace EduPulse.Repository.Abstracts;

public interface IParentRepository
{
    Task<List<Parent>> GetAllAsync();
    Task<List<Parent>> GetBySchoolIdAsync(string schoolId);
    Task<Parent?> GetByIdAsync(string id);
    Task CreateAsync(Parent parent);
    Task UpdateAsync(Parent parent);
    Task DeleteAsync(string id);
}