using EduPulse.Entities.TeacherLessons;

namespace EduPulse.Repository.Abstracts;

public interface ITeacherLessonRepository
{
    Task<List<TeacherLesson>> GetAllAsync();
    Task<List<TeacherLesson>> GetBySchoolIdAsync(string schoolId);
    Task<TeacherLesson?> GetByIdAsync(string id);

    Task<TeacherLesson?> GetDuplicateAsync(
        string schoolId,
        string teacherId,
        string lessonId,
        string classroomId);

    Task<TeacherLesson?> GetByTeacherLessonAndClassroomAsync(
        string teacherId,
        string lessonId,
        string classroomId);

    Task AddAsync(TeacherLesson teacherLesson);
    Task UpdateAsync(TeacherLesson teacherLesson);
    Task DeleteAsync(string id);
}