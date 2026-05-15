using EduPulse.Entities.TeacherLessons;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Context;
using MongoDB.Driver;

namespace EduPulse.Repository.Concretes;

public class TeacherLessonRepository : ITeacherLessonRepository
{
    private readonly IMongoCollection<TeacherLesson> _teacherLessons;

    public TeacherLessonRepository(MongoDbContext context)
    {
        _teacherLessons = context.TeacherLessons;
    }

    public async Task<List<TeacherLesson>> GetAllAsync()
    {
        return await _teacherLessons
            .Find(_ => true)
            .SortByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<TeacherLesson>> GetBySchoolIdAsync(string schoolId)
    {
        return await _teacherLessons
            .Find(x => x.SchoolId == schoolId)
            .SortByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<TeacherLesson>> GetByTeacherIdAsync(string teacherId)
    {
        return await _teacherLessons
            .Find(x => x.TeacherId == teacherId)
            .SortByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<TeacherLesson?> GetByIdAsync(string id)
    {
        return await _teacherLessons
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<TeacherLesson?> GetDuplicateAsync(
        string schoolId,
        string teacherId,
        string lessonId,
        string classroomId)
    {
        return await _teacherLessons
            .Find(x =>
                x.SchoolId == schoolId &&
                x.TeacherId == teacherId &&
                x.LessonId == lessonId &&
                x.ClassroomId == classroomId)
            .FirstOrDefaultAsync();
    }

    public async Task<TeacherLesson?> GetByTeacherLessonAndClassroomAsync(
        string teacherId,
        string lessonId,
        string classroomId)
    {
        return await _teacherLessons
            .Find(x =>
                x.TeacherId == teacherId &&
                x.LessonId == lessonId &&
                x.ClassroomId == classroomId)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(TeacherLesson teacherLesson)
    {
        teacherLesson.CreatedDate = DateTime.UtcNow;
        teacherLesson.UpdatedDate = null;

        await _teacherLessons.InsertOneAsync(teacherLesson);
    }

    public async Task UpdateAsync(TeacherLesson teacherLesson)
    {
        teacherLesson.UpdatedDate = DateTime.UtcNow;

        await _teacherLessons.ReplaceOneAsync(
            x => x.Id == teacherLesson.Id,
            teacherLesson
        );
    }

    public async Task DeleteAsync(string id)
    {
        await _teacherLessons.DeleteOneAsync(x => x.Id == id);
    }
}