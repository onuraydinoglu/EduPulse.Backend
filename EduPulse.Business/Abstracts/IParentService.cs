using EduPulse.DTOs.Common;
using EduPulse.DTOs.Parents;

namespace EduPulse.Business.Abstracts;

public interface IParentService
{
    Task<Result<List<ParentListDto>>> GetAllAsync();
    Task<Result<List<ParentListDto>>> GetBySchoolIdAsync(string schoolId);
    Task<Result<ParentListDto>> GetByIdAsync(string id);
    Task<Result> CreateAsync(CreateParentDto dto);
    Task<Result> UpdateAsync(UpdateParentDto dto);
    Task<Result> DeleteAsync(string id);
}