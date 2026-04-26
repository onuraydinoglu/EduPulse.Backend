using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Lessons;
using EduPulse.Entities.Lessons;
using EduPulse.Repository.Abstracts;

namespace EduPulse.Business.Concretes;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ISchoolRepository _schoolRepository;

    public LessonService(
        ILessonRepository lessonRepository,
        ISchoolRepository schoolRepository)
    {
        _lessonRepository = lessonRepository;
        _schoolRepository = schoolRepository;
    }

    public async Task<List<LessonListDto>> GetAllAsync()
    {
        var lessons = await _lessonRepository.GetAllAsync();

        return lessons.Select(x => new LessonListDto
        {
            Id = x.Id,
            SchoolId = x.SchoolId,
            Name = x.Name,
            IsActive = x.IsActive
        }).ToList();
    }

    public async Task<List<LessonListDto>> GetBySchoolIdAsync(string schoolId)
    {
        var lessons = await _lessonRepository.GetBySchoolIdAsync(schoolId);

        return lessons.Select(x => new LessonListDto
        {
            Id = x.Id,
            SchoolId = x.SchoolId,
            Name = x.Name,
            IsActive = x.IsActive
        }).ToList();
    }

    public async Task<LessonListDto?> GetByIdAsync(string id)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);

        if (lesson is null)
            return null;

        return new LessonListDto
        {
            Id = lesson.Id,
            SchoolId = lesson.SchoolId,
            Name = lesson.Name,
            IsActive = lesson.IsActive
        };
    }

    public async Task CreateAsync(CreateLessonDto dto)
    {
        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            throw new Exception("Okul bulunamadı.");

        var lesson = new Lesson
        {
            SchoolId = dto.SchoolId,
            Name = dto.Name,
            IsActive = true
        };

        await _lessonRepository.CreateAsync(lesson);
    }

    public async Task UpdateAsync(UpdateLessonDto dto)
    {
        var lesson = await _lessonRepository.GetByIdAsync(dto.Id);

        if (lesson is null)
            throw new Exception("Ders bulunamadı.");

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            throw new Exception("Okul bulunamadı.");

        lesson.SchoolId = dto.SchoolId;
        lesson.Name = dto.Name;
        lesson.IsActive = dto.IsActive;

        await _lessonRepository.UpdateAsync(lesson);
    }

    public async Task DeleteAsync(string id)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);

        if (lesson is null)
            throw new Exception("Ders bulunamadı.");

        await _lessonRepository.DeleteAsync(id);
    }
}