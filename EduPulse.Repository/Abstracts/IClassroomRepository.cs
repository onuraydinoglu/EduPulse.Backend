using EduPulse.Entities.Classrooms;

namespace EduPulse.Repository.Abstracts;

public interface IClassroomRepository
{
    Task<List<Classroom>> GetAllAsync();
    Task<List<Classroom>> GetBySchoolIdAsync(string schoolId);
    Task<Classroom?> GetByIdAsync(string id);

    Task<Classroom?> GetBySchoolIdAndTeacherIdAsync(string schoolId, string teacherId);
    Task<Classroom?> GetBySchoolIdAndTeacherIdExceptClassroomIdAsync(string schoolId, string teacherId, string classroomId);

    Task<Classroom?> GetBySchoolGradeSectionAsync(string schoolId, int grade, string section);
    Task CreateAsync(Classroom classroom);
    Task UpdateAsync(Classroom classroom);
    Task DeleteAsync(string id);
}