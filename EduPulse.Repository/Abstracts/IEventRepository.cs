using EduPulse.Entities.Events;

namespace EduPulse.Repository.Abstracts;

public interface IEventRepository
{
    Task<List<Event>> GetAllAsync();
    Task<List<Event>> GetBySchoolIdAsync(string schoolId);
    Task<Event?> GetByIdAsync(string id);
    Task<Event?> GetBySchoolIdAndNameAsync(string schoolId, string normalizedName);
    Task CreateAsync(Event eventEntity);
    Task UpdateAsync(Event eventEntity);
    Task DeleteAsync(string id);
}