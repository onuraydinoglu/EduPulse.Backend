using EduPulse.DTOs.Teachers;

namespace EduPulse.Business.Abstracts;

public interface ITeacherService
{
    Task<List<TeacherListDto>> GetAllAsync();
    Task<List<TeacherListDto>> GetBySchoolIdAsync(string schoolId);
    Task<TeacherListDto?> GetByIdAsync(string id);
    Task CreateAsync(CreateTeacherDto dto);
    Task UpdateAsync(UpdateTeacherDto dto);
    Task DeleteAsync(string id);
}