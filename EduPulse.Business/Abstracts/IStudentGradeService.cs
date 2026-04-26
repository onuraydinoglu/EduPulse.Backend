using EduPulse.DTOs.Common;
using EduPulse.DTOs.StudentGrades;

namespace EduPulse.Business.Abstracts;

public interface IStudentGradeService
{
    Task<Result<List<StudentGradeListDto>>> GetAllAsync();
    Task<Result<List<StudentGradeListDto>>> GetByStudentIdAsync(string studentId);
    Task<Result<List<StudentGradeListDto>>> GetByLessonIdAsync(string lessonId);
    Task<Result<StudentGradeListDto>> GetByIdAsync(string id);
    Task<Result> CreateAsync(CreateStudentGradeDto dto);
    Task<Result> UpdateAsync(UpdateStudentGradeDto dto);
    Task<Result> DeleteAsync(string id);
}