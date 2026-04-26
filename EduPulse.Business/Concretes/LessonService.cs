using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Common;
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

    public async Task<Result<List<LessonListDto>>> GetAllAsync()
    {
        var lessons = await _lessonRepository.GetAllAsync();

        var result = lessons.Select(x => new LessonListDto
        {
            Id = x.Id,
            SchoolId = x.SchoolId,
            Name = x.Name,
            IsActive = x.IsActive
        }).ToList();

        return Result<List<LessonListDto>>.Success(result, "Dersler başarıyla listelendi.");
    }

    public async Task<Result<List<LessonListDto>>> GetBySchoolIdAsync(string schoolId)
    {
        var school = await _schoolRepository.GetByIdAsync(schoolId);

        if (school is null)
            return Result<List<LessonListDto>>.Failure("Okul bulunamadı.", 404);

        var lessons = await _lessonRepository.GetBySchoolIdAsync(schoolId);

        var result = lessons.Select(x => new LessonListDto
        {
            Id = x.Id,
            SchoolId = x.SchoolId,
            Name = x.Name,
            IsActive = x.IsActive
        }).ToList();

        return Result<List<LessonListDto>>.Success(result, "Okula ait dersler başarıyla listelendi.");
    }

    public async Task<Result<LessonListDto>> GetByIdAsync(string id)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);

        if (lesson is null)
            return Result<LessonListDto>.Failure("Ders bulunamadı.", 404);

        var result = new LessonListDto
        {
            Id = lesson.Id,
            SchoolId = lesson.SchoolId,
            Name = lesson.Name,
            IsActive = lesson.IsActive
        };

        return Result<LessonListDto>.Success(result, "Ders başarıyla getirildi.");
    }

    public async Task<Result> CreateAsync(CreateLessonDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            return Result.Failure("Okul bulunamadı.", 404);

        var lesson = new Lesson
        {
            SchoolId = dto.SchoolId,
            Name = dto.Name,
            IsActive = true
        };

        await _lessonRepository.CreateAsync(lesson);

        return Result.Success("Ders başarıyla oluşturuldu.", 201);
    }

    public async Task<Result> UpdateAsync(UpdateLessonDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var lesson = await _lessonRepository.GetByIdAsync(dto.Id);

        if (lesson is null)
            return Result.Failure("Ders bulunamadı.", 404);

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            return Result.Failure("Okul bulunamadı.", 404);

        lesson.SchoolId = dto.SchoolId;
        lesson.Name = dto.Name;
        lesson.IsActive = dto.IsActive;

        await _lessonRepository.UpdateAsync(lesson);

        return Result.Success("Ders başarıyla güncellendi.");
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);

        if (lesson is null)
            return Result.Failure("Ders bulunamadı.", 404);

        await _lessonRepository.DeleteAsync(id);

        return Result.Success("Ders başarıyla silindi.");
    }
}