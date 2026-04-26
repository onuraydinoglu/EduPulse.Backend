using EduPulse.Entities.StudentGrades;

namespace EduPulse.Repository.Abstracts;

public interface IStudentGradeRepository
{
    Task<List<StudentGrade>> GetAllAsync();
    Task<List<StudentGrade>> GetBySchoolIdAsync(string schoolId);
    Task<List<StudentGrade>> GetByStudentIdAsync(string studentId);
    Task<List<StudentGrade>> GetByLessonIdAsync(string lessonId);
    Task<StudentGrade?> GetByIdAsync(string id);
    Task<StudentGrade?> GetByStudentAndLessonAsync(string studentId, string lessonId);
    Task CreateAsync(StudentGrade studentGrade);
    Task UpdateAsync(StudentGrade studentGrade);
    Task DeleteAsync(string id);
}