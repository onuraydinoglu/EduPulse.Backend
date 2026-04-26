using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Common;
using EduPulse.DTOs.TeacherLessons;
using EduPulse.Entities.TeacherLessons;
using EduPulse.Repository.Abstracts;
using FluentValidation;

namespace EduPulse.Business.Concretes;

public class TeacherLessonService : ITeacherLessonService
{
    private readonly ITeacherLessonRepository _teacherLessonRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IClassroomRepository _classroomRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly IValidator<CreateTeacherLessonDto> _createValidator;
    private readonly IValidator<UpdateTeacherLessonDto> _updateValidator;

    public TeacherLessonService(
        ITeacherLessonRepository teacherLessonRepository,
        ITeacherRepository teacherRepository,
        ILessonRepository lessonRepository,
        IClassroomRepository classroomRepository,
        ISchoolRepository schoolRepository,
        IValidator<CreateTeacherLessonDto> createValidator,
        IValidator<UpdateTeacherLessonDto> updateValidator)
    {
        _teacherLessonRepository = teacherLessonRepository;
        _teacherRepository = teacherRepository;
        _lessonRepository = lessonRepository;
        _classroomRepository = classroomRepository;
        _schoolRepository = schoolRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<List<TeacherLessonListDto>>> GetAllAsync()
    {
        var list = await _teacherLessonRepository.GetAllAsync();
        var result = await MapToListDtoAsync(list);

        return Result<List<TeacherLessonListDto>>.Success(result, "Öğretmen ders atamaları başarıyla listelendi.");
    }

    public async Task<Result<List<TeacherLessonListDto>>> GetBySchoolIdAsync(string schoolId)
    {
        var school = await _schoolRepository.GetByIdAsync(schoolId);

        if (school is null)
            return Result<List<TeacherLessonListDto>>.Failure("Okul bulunamadı.", 404);

        var list = await _teacherLessonRepository.GetBySchoolIdAsync(schoolId);
        var result = await MapToListDtoAsync(list);

        return Result<List<TeacherLessonListDto>>.Success(result, "Okula ait öğretmen ders atamaları başarıyla listelendi.");
    }

    public async Task<Result<List<TeacherLessonListDto>>> GetByTeacherIdAsync(string teacherId)
    {
        var teacher = await _teacherRepository.GetByIdAsync(teacherId);

        if (teacher is null)
            return Result<List<TeacherLessonListDto>>.Failure("Öğretmen bulunamadı.", 404);

        var list = await _teacherLessonRepository.GetByTeacherIdAsync(teacherId);
        var result = await MapToListDtoAsync(list);

        return Result<List<TeacherLessonListDto>>.Success(result, "Öğretmene ait ders atamaları başarıyla listelendi.");
    }

    public async Task<Result<TeacherLessonListDto>> GetByIdAsync(string id)
    {
        var x = await _teacherLessonRepository.GetByIdAsync(id);

        if (x is null)
            return Result<TeacherLessonListDto>.Failure("Kayıt bulunamadı.", 404);

        var teacher = await _teacherRepository.GetByIdAsync(x.TeacherId);
        var lesson = await _lessonRepository.GetByIdAsync(x.LessonId);
        var classroom = await _classroomRepository.GetByIdAsync(x.ClassroomId);

        var result = new TeacherLessonListDto
        {
            Id = x.Id,
            SchoolId = x.SchoolId,
            TeacherId = x.TeacherId,
            TeacherName = teacher != null ? $"{teacher.FirstName} {teacher.LastName}" : "",
            LessonId = x.LessonId,
            LessonName = lesson?.Name ?? "",
            ClassroomId = x.ClassroomId,
            ClassroomName = classroom != null ? $"{classroom.Grade}-{classroom.Section}" : "",
            IsActive = x.IsActive
        };

        return Result<TeacherLessonListDto>.Success(result, "Öğretmen ders ataması başarıyla getirildi.");
    }

    public async Task<Result> CreateAsync(CreateTeacherLessonDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            return Result.Failure("Okul bulunamadı.", 404);

        var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);

        if (teacher is null)
            return Result.Failure("Öğretmen bulunamadı.", 404);

        var lesson = await _lessonRepository.GetByIdAsync(dto.LessonId);

        if (lesson is null)
            return Result.Failure("Ders bulunamadı.", 404);

        var classroom = await _classroomRepository.GetByIdAsync(dto.ClassroomId);

        if (classroom is null)
            return Result.Failure("Sınıf bulunamadı.", 404);

        if (teacher.SchoolId != dto.SchoolId)
            return Result.Failure("Seçilen öğretmen bu okula ait değil.", 400);

        if (lesson.SchoolId != dto.SchoolId)
            return Result.Failure("Seçilen ders bu okula ait değil.", 400);

        if (classroom.SchoolId != dto.SchoolId)
            return Result.Failure("Seçilen sınıf bu okula ait değil.", 400);

        var entity = new TeacherLesson
        {
            SchoolId = dto.SchoolId,
            TeacherId = dto.TeacherId,
            LessonId = dto.LessonId,
            ClassroomId = dto.ClassroomId,
            IsActive = true
        };

        await _teacherLessonRepository.CreateAsync(entity);

        return Result.Success("Öğretmen ders ataması başarıyla oluşturuldu.", 201);
    }

    public async Task<Result> UpdateAsync(UpdateTeacherLessonDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var entity = await _teacherLessonRepository.GetByIdAsync(dto.Id);

        if (entity is null)
            return Result.Failure("Kayıt bulunamadı.", 404);

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            return Result.Failure("Okul bulunamadı.", 404);

        var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);

        if (teacher is null)
            return Result.Failure("Öğretmen bulunamadı.", 404);

        var lesson = await _lessonRepository.GetByIdAsync(dto.LessonId);

        if (lesson is null)
            return Result.Failure("Ders bulunamadı.", 404);

        var classroom = await _classroomRepository.GetByIdAsync(dto.ClassroomId);

        if (classroom is null)
            return Result.Failure("Sınıf bulunamadı.", 404);

        if (teacher.SchoolId != dto.SchoolId)
            return Result.Failure("Seçilen öğretmen bu okula ait değil.", 400);

        if (lesson.SchoolId != dto.SchoolId)
            return Result.Failure("Seçilen ders bu okula ait değil.", 400);

        if (classroom.SchoolId != dto.SchoolId)
            return Result.Failure("Seçilen sınıf bu okula ait değil.", 400);

        entity.SchoolId = dto.SchoolId;
        entity.TeacherId = dto.TeacherId;
        entity.LessonId = dto.LessonId;
        entity.ClassroomId = dto.ClassroomId;
        entity.IsActive = dto.IsActive;

        await _teacherLessonRepository.UpdateAsync(entity);

        return Result.Success("Öğretmen ders ataması başarıyla güncellendi.");
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var entity = await _teacherLessonRepository.GetByIdAsync(id);

        if (entity is null)
            return Result.Failure("Kayıt bulunamadı.", 404);

        await _teacherLessonRepository.DeleteAsync(id);

        return Result.Success("Öğretmen ders ataması başarıyla silindi.");
    }

    private async Task<List<TeacherLessonListDto>> MapToListDtoAsync(List<TeacherLesson> list)
    {
        var teachers = await _teacherRepository.GetAllAsync();
        var lessons = await _lessonRepository.GetAllAsync();
        var classrooms = await _classroomRepository.GetAllAsync();

        return list.Select(x =>
        {
            var teacher = teachers.FirstOrDefault(t => t.Id == x.TeacherId);
            var lesson = lessons.FirstOrDefault(l => l.Id == x.LessonId);
            var classroom = classrooms.FirstOrDefault(c => c.Id == x.ClassroomId);

            return new TeacherLessonListDto
            {
                Id = x.Id,
                SchoolId = x.SchoolId,
                TeacherId = x.TeacherId,
                TeacherName = teacher != null ? $"{teacher.FirstName} {teacher.LastName}" : "",
                LessonId = x.LessonId,
                LessonName = lesson?.Name ?? "",
                ClassroomId = x.ClassroomId,
                ClassroomName = classroom != null ? $"{classroom.Grade}-{classroom.Section}" : "",
                IsActive = x.IsActive
            };
        }).ToList();
    }
}