using EduPulse.DTOs.Schools;

namespace EduPulse.Business.Abstracts;

public interface ISchoolService
{
    Task<List<SchoolListDto>> GetAllAsync();
    Task<SchoolListDto?> GetByIdAsync(string id);
    Task CreateAsync(CreateSchoolDto dto);
    Task UpdateAsync(UpdateSchoolDto dto);
    Task DeleteAsync(string id);
}