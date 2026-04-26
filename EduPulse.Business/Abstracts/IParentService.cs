using EduPulse.DTOs.Parents;

namespace EduPulse.Business.Abstracts;

public interface IParentService
{
    Task<List<ParentListDto>> GetAllAsync();
    Task<List<ParentListDto>> GetBySchoolIdAsync(string schoolId);
    Task<ParentListDto?> GetByIdAsync(string id);
    Task CreateAsync(CreateParentDto dto);
    Task UpdateAsync(UpdateParentDto dto);
    Task DeleteAsync(string id);
}