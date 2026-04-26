using EduPulse.Entities.Classrooms;

namespace EduPulse.Repository.Abstracts;

public interface IClassroomRepository
{
    Task<List<Classroom>> GetAllAsync();
    Task<List<Classroom>> GetBySchoolIdAsync(string schoolId);
    Task<Classroom?> GetByIdAsync(string id);
    Task CreateAsync(Classroom classroom);
    Task UpdateAsync(Classroom classroom);
    Task DeleteAsync(string id);
}