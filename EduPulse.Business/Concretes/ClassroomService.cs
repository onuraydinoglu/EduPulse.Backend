using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Classrooms;
using EduPulse.DTOs.Common;
using EduPulse.Entities.Classrooms;
using EduPulse.Repository.Abstracts;
using FluentValidation;

namespace EduPulse.Business.Concretes;

public class ClassroomService : IClassroomService
{
    private readonly IClassroomRepository _classroomRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IValidator<CreateClassroomDto> _createValidator;
    private readonly IValidator<UpdateClassroomDto> _updateValidator;

    public ClassroomService(
        IClassroomRepository classroomRepository,
        ISchoolRepository schoolRepository,
        ITeacherRepository teacherRepository,
        IValidator<CreateClassroomDto> createValidator,
        IValidator<UpdateClassroomDto> updateValidator)
    {
        _classroomRepository = classroomRepository;
        _schoolRepository = schoolRepository;
        _teacherRepository = teacherRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<List<ClassroomListDto>>> GetAllAsync()
    {
        var classrooms = await _classroomRepository.GetAllAsync();
        var teachers = await _teacherRepository.GetAllAsync();

        var result = classrooms.Select(x =>
        {
            var teacher = teachers.FirstOrDefault(t => t.Id == x.TeacherId);

            return new ClassroomListDto
            {
                Id = x.Id,
                SchoolId = x.SchoolId,
                Grade = x.Grade,
                Section = x.Section,
                TeacherId = x.TeacherId,
                TeacherName = teacher != null
                    ? $"{teacher.FirstName} {teacher.LastName}"
                    : null,
                IsActive = x.IsActive
            };
        }).ToList();

        return Result<List<ClassroomListDto>>.Success(result, "Sınıflar başarıyla listelendi.");
    }

    public async Task<Result<List<ClassroomListDto>>> GetBySchoolIdAsync(string schoolId)
    {
        var school = await _schoolRepository.GetByIdAsync(schoolId);

        if (school is null)
            return Result<List<ClassroomListDto>>.Failure("Okul bulunamadı.", 404);

        var classrooms = await _classroomRepository.GetBySchoolIdAsync(schoolId);
        var teachers = await _teacherRepository.GetAllAsync();

        var result = classrooms.Select(x =>
        {
            var teacher = teachers.FirstOrDefault(t => t.Id == x.TeacherId);

            return new ClassroomListDto
            {
                Id = x.Id,
                SchoolId = x.SchoolId,
                Grade = x.Grade,
                Section = x.Section,
                TeacherId = x.TeacherId,
                TeacherName = teacher != null
                    ? $"{teacher.FirstName} {teacher.LastName}"
                    : null,
                IsActive = x.IsActive
            };
        }).ToList();

        return Result<List<ClassroomListDto>>.Success(result, "Okula ait sınıflar başarıyla listelendi.");
    }

    public async Task<Result<ClassroomListDto>> GetByIdAsync(string id)
    {
        var classroom = await _classroomRepository.GetByIdAsync(id);

        if (classroom is null)
            return Result<ClassroomListDto>.Failure("Sınıf bulunamadı.", 404);

        var teacher = !string.IsNullOrWhiteSpace(classroom.TeacherId)
            ? await _teacherRepository.GetByIdAsync(classroom.TeacherId)
            : null;

        var result = new ClassroomListDto
        {
            Id = classroom.Id,
            SchoolId = classroom.SchoolId,
            Grade = classroom.Grade,
            Section = classroom.Section,
            TeacherId = classroom.TeacherId,
            TeacherName = teacher != null
                ? $"{teacher.FirstName} {teacher.LastName}"
                : null,
            IsActive = classroom.IsActive
        };

        return Result<ClassroomListDto>.Success(result, "Sınıf başarıyla getirildi.");
    }

    public async Task<Result> CreateAsync(CreateClassroomDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            return Result.Failure("Okul bulunamadı.", 404);

        if (!string.IsNullOrWhiteSpace(dto.TeacherId))
        {
            var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);

            if (teacher is null)
                return Result.Failure("Öğretmen bulunamadı.", 404);
        }

        var classroom = new Classroom
        {
            SchoolId = dto.SchoolId,
            Grade = dto.Grade,
            Section = dto.Section,
            TeacherId = dto.TeacherId,
            IsActive = true
        };

        await _classroomRepository.CreateAsync(classroom);

        return Result.Success("Sınıf başarıyla oluşturuldu.", 201);
    }

    public async Task<Result> UpdateAsync(UpdateClassroomDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var classroom = await _classroomRepository.GetByIdAsync(dto.Id);

        if (classroom is null)
            return Result.Failure("Sınıf bulunamadı.", 404);

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            return Result.Failure("Okul bulunamadı.", 404);

        if (!string.IsNullOrWhiteSpace(dto.TeacherId))
        {
            var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);

            if (teacher is null)
                return Result.Failure("Öğretmen bulunamadı.", 404);
        }

        classroom.SchoolId = dto.SchoolId;
        classroom.Grade = dto.Grade;
        classroom.Section = dto.Section;
        classroom.TeacherId = dto.TeacherId;
        classroom.IsActive = dto.IsActive;

        await _classroomRepository.UpdateAsync(classroom);

        return Result.Success("Sınıf başarıyla güncellendi.");
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var classroom = await _classroomRepository.GetByIdAsync(id);

        if (classroom is null)
            return Result.Failure("Sınıf bulunamadı.", 404);

        await _classroomRepository.DeleteAsync(id);

        return Result.Success("Sınıf başarıyla silindi.");
    }
}