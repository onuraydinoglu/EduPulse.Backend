using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Lessons;
using EduPulse.Entities.Lessons;
using EduPulse.Repository.Abstracts;
using FluentValidation;

namespace EduPulse.Business.Concretes;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly IValidator<CreateLessonDto> _createValidator;
    private readonly IValidator<UpdateLessonDto> _updateValidator;

    public LessonService(
        ILessonRepository lessonRepository,
        ISchoolRepository schoolRepository,
        IValidator<CreateLessonDto> createValidator,
        IValidator<UpdateLessonDto> updateValidator)
    {
        _lessonRepository = lessonRepository;
        _schoolRepository = schoolRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
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
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            throw new ArgumentException(validationResult.Errors.First().ErrorMessage);

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            throw new ArgumentException("Okul bulunamadı.");

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
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            throw new ArgumentException(validationResult.Errors.First().ErrorMessage);

        var lesson = await _lessonRepository.GetByIdAsync(dto.Id);

        if (lesson is null)
            throw new ArgumentException("Ders bulunamadı.");

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            throw new ArgumentException("Okul bulunamadı.");

        lesson.SchoolId = dto.SchoolId;
        lesson.Name = dto.Name;
        lesson.IsActive = dto.IsActive;

        await _lessonRepository.UpdateAsync(lesson);
    }

    public async Task DeleteAsync(string id)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);

        if (lesson is null)
            throw new ArgumentException("Ders bulunamadı.");

        await _lessonRepository.DeleteAsync(id);
    }
}