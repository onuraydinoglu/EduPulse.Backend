using EduPulse.DTOs.Classrooms;

namespace EduPulse.Business.Abstracts;

public interface IClassroomService
{
    Task<List<ClassroomListDto>> GetAllAsync();
    Task<List<ClassroomListDto>> GetBySchoolIdAsync(string schoolId);
    Task<ClassroomListDto?> GetByIdAsync(string id);
    Task CreateAsync(CreateClassroomDto dto);
    Task UpdateAsync(UpdateClassroomDto dto);
    Task DeleteAsync(string id);
}