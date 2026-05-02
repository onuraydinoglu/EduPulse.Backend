using EduPulse.Entities.Clubs;

namespace EduPulse.Repository.Abstracts;

public interface IClubRepository
{
    Task<List<Club>> GetAllAsync();
    Task<List<Club>> GetBySchoolIdAsync(string schoolId);
    Task<Club?> GetByIdAsync(string id);
    Task<Club?> GetBySchoolIdAndNameAsync(string schoolId, string normalizedName);
    Task CreateAsync(Club club);
    Task UpdateAsync(Club club);
    Task DeleteAsync(string id);
}