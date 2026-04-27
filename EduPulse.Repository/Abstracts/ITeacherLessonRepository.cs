using EduPulse.Entities.TeacherLessons;

namespace EduPulse.Repository.Abstracts;

public interface ITeacherLessonRepository
{
    Task<List<TeacherLesson>> GetAllAsync();
    Task<List<TeacherLesson>> GetBySchoolIdAsync(string schoolId);
    Task<List<TeacherLesson>> GetByTeacherIdAsync(string teacherId);
    Task<TeacherLesson?> GetByIdAsync(string id);

    Task<TeacherLesson?> GetByTeacherLessonAndClassroomAsync(
        string teacherId,
        string lessonId,
        string classroomId);

    Task CreateAsync(TeacherLesson teacherLesson);
    Task UpdateAsync(TeacherLesson teacherLesson);
    Task DeleteAsync(string id);
}