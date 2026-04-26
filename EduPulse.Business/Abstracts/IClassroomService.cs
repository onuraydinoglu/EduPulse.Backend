using EduPulse.DTOs.Classrooms;
using EduPulse.DTOs.Common;

namespace EduPulse.Business.Abstracts;

public interface IClassroomService
{
    Task<Result<List<ClassroomListDto>>> GetAllAsync();
    Task<Result<List<ClassroomListDto>>> GetBySchoolIdAsync(string schoolId);
    Task<Result<ClassroomListDto>> GetByIdAsync(string id);
    Task<Result> CreateAsync(CreateClassroomDto dto);
    Task<Result> UpdateAsync(UpdateClassroomDto dto);
    Task<Result> DeleteAsync(string id);
}