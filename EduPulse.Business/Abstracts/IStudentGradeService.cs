using EduPulse.DTOs.StudentGrades;

namespace EduPulse.Business.Abstracts;

public interface IStudentGradeService
{
    Task<List<StudentGradeListDto>> GetAllAsync();
    Task<List<StudentGradeListDto>> GetByStudentIdAsync(string studentId);
    Task<List<StudentGradeListDto>> GetByLessonIdAsync(string lessonId);
    Task CreateAsync(CreateStudentGradeDto dto);
    Task UpdateAsync(UpdateStudentGradeDto dto);
    Task DeleteAsync(string id);
}