using EduPulse.DTOs.Common;
using EduPulse.DTOs.Students;

namespace EduPulse.Business.Abstracts;

public interface IStudentService
{
    Task<Result<List<StudentListDto>>> GetAllAsync();
    Task<Result<List<StudentListDto>>> GetBySchoolIdAsync(string schoolId);
    Task<Result<List<StudentListDto>>> GetByClassroomIdAsync(string classroomId);
    Task<Result<StudentListDto>> GetByIdAsync(string id);
    Task<Result> CreateAsync(CreateStudentDto dto);
    Task<Result> UpdateAsync(UpdateStudentDto dto);
    Task<Result> DeleteAsync(string id);
}