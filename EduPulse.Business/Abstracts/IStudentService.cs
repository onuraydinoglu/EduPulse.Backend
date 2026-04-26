using EduPulse.DTOs.Students;

namespace EduPulse.Business.Abstracts;

public interface IStudentService
{
    Task<List<StudentListDto>> GetAllAsync();
    Task<List<StudentListDto>> GetBySchoolIdAsync(string schoolId);
    Task<List<StudentListDto>> GetByClassroomIdAsync(string classroomId);
    Task<StudentListDto?> GetByIdAsync(string id);
    Task CreateAsync(CreateStudentDto dto);
    Task UpdateAsync(UpdateStudentDto dto);
    Task DeleteAsync(string id);
}