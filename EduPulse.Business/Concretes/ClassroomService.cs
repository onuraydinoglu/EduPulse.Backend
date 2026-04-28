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
    private readonly IUserRepository _userRepository;
    private readonly IValidator<CreateClassroomDto> _createValidator;
    private readonly IValidator<UpdateClassroomDto> _updateValidator;

    public ClassroomService(
        IClassroomRepository classroomRepository,
        IUserRepository userRepository,
        IValidator<CreateClassroomDto> createValidator,
        IValidator<UpdateClassroomDto> updateValidator)
    {
        _classroomRepository = classroomRepository;
        _userRepository = userRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<List<ClassroomListDto>>> GetAllForCurrentUserAsync(string? roleName, string? schoolId)
    {
        List<Classroom> classrooms;

        if (roleName == "superadmin")
        {
            classrooms = await _classroomRepository.GetAllAsync();
        }
        else
        {
            if (string.IsNullOrWhiteSpace(schoolId))
                return Result<List<ClassroomListDto>>.Failure("Okul bilgisi bulunamadı.", 400);

            classrooms = await _classroomRepository.GetBySchoolIdAsync(schoolId);
        }

        var dtoList = classrooms.Select(MapToListDto).ToList();

        return Result<List<ClassroomListDto>>.Success(dtoList, "Sınıflar başarıyla listelendi.", 200);
    }

    public async Task<Result<ClassroomListDto>> GetByIdForCurrentUserAsync(string id, string? roleName, string? schoolId)
    {
        var classroom = await _classroomRepository.GetByIdAsync(id);

        if (classroom is null)
            return Result<ClassroomListDto>.Failure("Sınıf bulunamadı.", 404);

        if (roleName != "superadmin" && classroom.SchoolId != schoolId)
            return Result<ClassroomListDto>.Failure("Bu sınıfa erişim yetkiniz yok.", 403);

        return Result<ClassroomListDto>.Success(MapToListDto(classroom), "Sınıf başarıyla getirildi.", 200);
    }

    public async Task<Result> CreateAsync(CreateClassroomDto dto, string? roleName, string? schoolId)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        if (roleName != "schooladmin")
            return Result.Failure("Sınıf ekleme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var selectedSchoolId = schoolId;
        var normalizedSection = dto.Section.Trim().ToUpper();

        var existingClassroom = await _classroomRepository.GetBySchoolGradeSectionAsync(
            selectedSchoolId,
            dto.Grade,
            normalizedSection
        );

        if (existingClassroom is not null)
            return Result.Failure("Bu okulda aynı sınıf zaten mevcut.", 400);

        if (!string.IsNullOrWhiteSpace(dto.TeacherId))
        {
            var teacher = await _userRepository.GetByIdAsync(dto.TeacherId);

            if (teacher is null)
                return Result.Failure("Öğretmen bulunamadı.", 404);

            if (teacher.SchoolId != selectedSchoolId)
                return Result.Failure("Seçilen öğretmen bu okula ait değil.", 400);
        }

        var classroom = new Classroom
        {
            SchoolId = selectedSchoolId,
            Grade = dto.Grade,
            Section = normalizedSection,
            TeacherId = dto.TeacherId,
            IsActive = true
        };

        await _classroomRepository.CreateAsync(classroom);

        return Result.Success("Sınıf başarıyla oluşturuldu.", 201);
    }

    public async Task<Result> UpdateAsync(UpdateClassroomDto dto, string? roleName, string? schoolId)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var classroom = await _classroomRepository.GetByIdAsync(dto.Id);

        if (classroom is null)
            return Result.Failure("Sınıf bulunamadı.", 404);

        if (roleName != "superadmin" && classroom.SchoolId != schoolId)
            return Result.Failure("Bu sınıfı güncelleme yetkiniz yok.", 403);

        var normalizedSection = dto.Section.Trim().ToUpper();

        var existingClassroom = await _classroomRepository.GetBySchoolGradeSectionAsync(
            classroom.SchoolId,
            dto.Grade,
            normalizedSection
        );

        if (existingClassroom is not null && existingClassroom.Id != dto.Id)
            return Result.Failure("Bu okulda aynı sınıf zaten mevcut.", 400);

        if (!string.IsNullOrWhiteSpace(dto.TeacherId))
        {
            var teacher = await _userRepository.GetByIdAsync(dto.TeacherId);

            if (teacher is null)
                return Result.Failure("Öğretmen bulunamadı.", 404);

            if (teacher.SchoolId != classroom.SchoolId)
                return Result.Failure("Seçilen öğretmen bu okula ait değil.", 400);
        }

        classroom.Grade = dto.Grade;
        classroom.Section = normalizedSection;
        classroom.TeacherId = dto.TeacherId;
        classroom.IsActive = dto.IsActive;

        await _classroomRepository.UpdateAsync(classroom);

        return Result.Success("Sınıf başarıyla güncellendi.", 200);
    }

    public async Task<Result> DeleteAsync(string id, string? roleName, string? schoolId)
    {
        var classroom = await _classroomRepository.GetByIdAsync(id);

        if (classroom is null)
            return Result.Failure("Sınıf bulunamadı.", 404);

        if (roleName != "superadmin" && classroom.SchoolId != schoolId)
            return Result.Failure("Bu sınıfı silme yetkiniz yok.", 403);

        await _classroomRepository.DeleteAsync(id);

        return Result.Success("Sınıf başarıyla silindi.", 200);
    }

    private static ClassroomListDto MapToListDto(Classroom classroom)
    {
        return new ClassroomListDto
        {
            Id = classroom.Id,
            SchoolId = classroom.SchoolId,
            Grade = classroom.Grade,
            Section = classroom.Section,
            TeacherId = classroom.TeacherId,
            IsActive = classroom.IsActive
        };
    }
}