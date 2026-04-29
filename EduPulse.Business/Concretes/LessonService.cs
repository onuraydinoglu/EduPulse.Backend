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
    private readonly IValidator<CreateLessonDto> _createValidator;
    private readonly IValidator<UpdateLessonDto> _updateValidator;

    public LessonService(
        ILessonRepository lessonRepository,
        IValidator<CreateLessonDto> createValidator,
        IValidator<UpdateLessonDto> updateValidator)
    {
        _lessonRepository = lessonRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<List<LessonListDto>>> GetAllForCurrentUserAsync(string? roleName, string? schoolId)
    {
        List<Lesson> lessons;

        if (roleName == "superadmin")
        {
            lessons = await _lessonRepository.GetAllAsync();
        }
        else
        {
            if (string.IsNullOrWhiteSpace(schoolId))
                return Result<List<LessonListDto>>.Failure("Okul bilgisi bulunamadı.", 400);

            lessons = await _lessonRepository.GetBySchoolIdAsync(schoolId);
        }

        var dtoList = lessons.Select(MapToListDto).ToList();

        return Result<List<LessonListDto>>.Success(dtoList, "Dersler başarıyla listelendi.", 200);
    }

    public async Task<Result<LessonListDto>> GetByIdForCurrentUserAsync(string id, string? roleName, string? schoolId)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);

        if (lesson is null || !lesson.IsActive)
            return Result<LessonListDto>.Failure("Ders bulunamadı.", 404);

        if (roleName != "superadmin" && lesson.SchoolId != schoolId)
            return Result<LessonListDto>.Failure("Bu derse erişim yetkiniz yok.", 403);

        return Result<LessonListDto>.Success(MapToListDto(lesson), "Ders başarıyla getirildi.", 200);
    }

    public async Task<Result> CreateAsync(CreateLessonDto dto, string? roleName, string? schoolId)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        if (roleName != "schooladmin" && roleName != "officer")
            return Result.Failure("Ders ekleme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var normalizedName = NormalizeLessonName(dto.Name);

        var existingLesson = await _lessonRepository
            .GetBySchoolIdAndNormalizedNameAsync(schoolId, normalizedName);

        if (existingLesson is not null)
            return Result.Failure("Bu okulda aynı ders zaten mevcut.", 400);

        var lesson = new Lesson
        {
            SchoolId = schoolId,
            Name = dto.Name.Trim(),
            NormalizedName = normalizedName,
            IsActive = true
        };

        await _lessonRepository.CreateAsync(lesson);

        return Result.Success("Ders başarıyla oluşturuldu.", 201);
    }

    public async Task<Result> UpdateAsync(UpdateLessonDto dto, string? roleName, string? schoolId)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        if (roleName != "schooladmin" && roleName != "officer")
            return Result.Failure("Ders güncelleme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var lesson = await _lessonRepository.GetByIdAsync(dto.Id);

        if (lesson is null || !lesson.IsActive)
            return Result.Failure("Ders bulunamadı.", 404);

        if (lesson.SchoolId != schoolId)
            return Result.Failure("Bu dersi güncelleme yetkiniz yok.", 403);

        var normalizedName = NormalizeLessonName(dto.Name);

        var existingLesson = await _lessonRepository
            .GetBySchoolIdAndNormalizedNameAsync(schoolId, normalizedName);

        if (existingLesson is not null && existingLesson.Id != dto.Id)
            return Result.Failure("Bu okulda aynı ders zaten mevcut.", 400);

        lesson.Name = dto.Name.Trim();
        lesson.NormalizedName = normalizedName;
        lesson.IsActive = dto.IsActive;

        await _lessonRepository.UpdateAsync(lesson);

        return Result.Success("Ders başarıyla güncellendi.", 200);
    }

    public async Task<Result> DeleteAsync(string id, string? roleName, string? schoolId)
    {
        if (roleName != "schooladmin" && roleName != "officer")
            return Result.Failure("Ders silme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var lesson = await _lessonRepository.GetByIdAsync(id);

        if (lesson is null || !lesson.IsActive)
            return Result.Failure("Ders bulunamadı.", 404);

        if (lesson.SchoolId != schoolId)
            return Result.Failure("Bu dersi silme yetkiniz yok.", 403);

        await _lessonRepository.DeleteAsync(id);

        return Result.Success("Ders başarıyla silindi.", 200);
    }

    private static LessonListDto MapToListDto(Lesson lesson)
    {
        return new LessonListDto
        {
            Id = lesson.Id,
            SchoolId = lesson.SchoolId,
            Name = lesson.Name,
            IsActive = lesson.IsActive
        };
    }

    private static string NormalizeLessonName(string name)
    {
        return name.Trim().ToUpperInvariant();
    }
}