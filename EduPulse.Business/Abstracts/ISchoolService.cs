using EduPulse.DTOs.Common;
using EduPulse.DTOs.Schools;

namespace EduPulse.Business.Abstracts;

public interface ISchoolService
{
    Task<Result<List<SchoolListDto>>> GetAllAsync();
    Task<Result<SchoolListDto>> GetByIdAsync(string id);
    Task<Result> CreateAsync(CreateSchoolDto dto);
    Task<Result> UpdateAsync(UpdateSchoolDto dto);
    Task<Result> DeleteAsync(string id);
}