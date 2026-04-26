using EduPulse.DTOs.Common;
using EduPulse.DTOs.Teachers;

namespace EduPulse.Business.Abstracts;

public interface ITeacherService
{
    Task<Result<List<TeacherListDto>>> GetAllAsync();
    Task<Result<List<TeacherListDto>>> GetBySchoolIdAsync(string schoolId);
    Task<Result<TeacherListDto>> GetByIdAsync(string id);
    Task<Result> CreateAsync(CreateTeacherDto dto);
    Task<Result> UpdateAsync(UpdateTeacherDto dto);
    Task<Result> DeleteAsync(string id);
}