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
    private readonly ITeacherRepository _teacherRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITeacherLessonRepository _teacherLessonRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IValidator<CreateClassroomDto> _createValidator;
    private readonly IValidator<UpdateClassroomDto> _updateValidator;

    public ClassroomService(
        IClassroomRepository classroomRepository,
        ITeacherRepository teacherRepository,
        IUserRepository userRepository,
        ITeacherLessonRepository teacherLessonRepository,
        IStudentRepository studentRepository,
        IValidator<CreateClassroomDto> createValidator,
        IValidator<UpdateClassroomDto> updateValidator)
    {
        _classroomRepository = classroomRepository;
        _teacherRepository = teacherRepository;
        _userRepository = userRepository;
        _teacherLessonRepository = teacherLessonRepository;
        _studentRepository = studentRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<List<ClassroomListDto>>> GetAllForCurrentUserAsync(
        string? roleName,
        string? schoolId,
        string? currentUserId)
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

            if (roleName == "teacher")
            {
                if (string.IsNullOrWhiteSpace(currentUserId))
                    return Result<List<ClassroomListDto>>.Failure("Kullanıcı bilgisi bulunamadı.", 400);

                var currentTeacher = await _teacherRepository.GetByUserIdAsync(currentUserId);

                if (currentTeacher is null)
                    return Result<List<ClassroomListDto>>.Failure("Öğretmen kaydı bulunamadı.", 404);

                if (currentTeacher.SchoolId != schoolId)
                    return Result<List<ClassroomListDto>>.Failure("Bu okula erişim yetkiniz yok.", 403);

                var allSchoolClassrooms = await _classroomRepository.GetBySchoolIdAsync(schoolId);
                var teacherLessons = await _teacherLessonRepository.GetByTeacherIdAsync(currentTeacher.Id);

                var allowedClassroomIds = teacherLessons
                    .Where(x => x.IsActive)
                    .Select(x => x.ClassroomId)
                    .ToHashSet();

                classrooms = allSchoolClassrooms
                    .Where(x =>
                        x.TeacherId == currentTeacher.Id ||
                        allowedClassroomIds.Contains(x.Id))
                    .OrderBy(x => x.Grade)
                    .ThenBy(x => x.Section)
                    .ToList();
            }
            else
            {
                classrooms = await _classroomRepository.GetBySchoolIdAsync(schoolId);
            }
        }

        var dtoList = new List<ClassroomListDto>();

        foreach (var classroom in classrooms)
        {
            dtoList.Add(await MapToListDtoAsync(classroom));
        }

        return Result<List<ClassroomListDto>>.Success(
            dtoList,
            "Sınıflar başarıyla listelendi.",
            200
        );
    }

    public async Task<Result<ClassroomListDto>> GetByIdForCurrentUserAsync(
        string id,
        string? roleName,
        string? schoolId,
        string? currentUserId)
    {
        var classroom = await _classroomRepository.GetByIdAsync(id);

        if (classroom is null)
            return Result<ClassroomListDto>.Failure("Sınıf bulunamadı.", 404);

        if (roleName != "superadmin")
        {
            if (string.IsNullOrWhiteSpace(schoolId))
                return Result<ClassroomListDto>.Failure("Okul bilgisi bulunamadı.", 400);

            if (classroom.SchoolId != schoolId)
                return Result<ClassroomListDto>.Failure("Bu sınıfa erişim yetkiniz yok.", 403);
        }

        if (roleName == "teacher")
        {
            var canAccess = await CanTeacherAccessClassroomAsync(
                classroom.Id,
                classroom.TeacherId,
                currentUserId,
                schoolId
            );

            if (!canAccess)
                return Result<ClassroomListDto>.Failure("Bu sınıfa erişim yetkiniz yok.", 403);
        }

        var dto = await MapToListDtoAsync(classroom);

        return Result<ClassroomListDto>.Success(
            dto,
            "Sınıf başarıyla getirildi.",
            200
        );
    }

    public async Task<Result> CreateAsync(
        CreateClassroomDto dto,
        string? roleName,
        string? schoolId)
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

        var normalizedTeacherId = string.IsNullOrWhiteSpace(dto.TeacherId)
            ? null
            : dto.TeacherId.Trim();

        if (!string.IsNullOrWhiteSpace(normalizedTeacherId))
        {
            var teacher = await _teacherRepository.GetByIdAsync(normalizedTeacherId);

            if (teacher is null)
                return Result.Failure("Öğretmen bulunamadı.", 404);

            if (teacher.SchoolId != selectedSchoolId)
                return Result.Failure("Seçilen öğretmen bu okula ait değil.", 400);

            var teacherClassroom = await _classroomRepository
                .GetBySchoolIdAndTeacherIdAsync(selectedSchoolId, normalizedTeacherId);

            if (teacherClassroom is not null)
                return Result.Failure("Bu öğretmen bu okulda zaten başka bir sınıfa atanmış.", 400);
        }

        var classroom = new Classroom
        {
            SchoolId = selectedSchoolId,
            Grade = dto.Grade,
            Section = normalizedSection,
            TeacherId = normalizedTeacherId,
            IsActive = true
        };

        await _classroomRepository.CreateAsync(classroom);

        return Result.Success("Sınıf başarıyla oluşturuldu.", 201);
    }

    public async Task<Result> UpdateAsync(
        UpdateClassroomDto dto,
        string? roleName,
        string? schoolId)
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

        var normalizedTeacherId = string.IsNullOrWhiteSpace(dto.TeacherId)
            ? null
            : dto.TeacherId.Trim();

        if (!string.IsNullOrWhiteSpace(normalizedTeacherId))
        {
            var teacher = await _teacherRepository.GetByIdAsync(normalizedTeacherId);

            if (teacher is null)
                return Result.Failure("Öğretmen bulunamadı.", 404);

            if (teacher.SchoolId != classroom.SchoolId)
                return Result.Failure("Seçilen öğretmen bu okula ait değil.", 400);

            var teacherClassroom = await _classroomRepository
                .GetBySchoolIdAndTeacherIdExceptClassroomIdAsync(
                    classroom.SchoolId,
                    normalizedTeacherId,
                    dto.Id
                );

            if (teacherClassroom is not null)
                return Result.Failure("Bu öğretmen bu okulda zaten başka bir sınıfa atanmış.", 400);
        }

        classroom.Grade = dto.Grade;
        classroom.Section = normalizedSection;
        classroom.TeacherId = normalizedTeacherId;
        classroom.IsActive = dto.IsActive;

        await _classroomRepository.UpdateAsync(classroom);

        return Result.Success("Sınıf başarıyla güncellendi.", 200);
    }

    public async Task<Result> DeleteAsync(
        string id,
        string? roleName,
        string? schoolId)
    {
        var classroom = await _classroomRepository.GetByIdAsync(id);

        if (classroom is null)
            return Result.Failure("Sınıf bulunamadı.", 404);

        if (roleName != "superadmin" && classroom.SchoolId != schoolId)
            return Result.Failure("Bu sınıfı silme yetkiniz yok.", 403);

        await _classroomRepository.DeleteAsync(id);

        return Result.Success("Sınıf başarıyla silindi.", 200);
    }

    private async Task<bool> CanTeacherAccessClassroomAsync(
        string classroomId,
        string? classroomAdvisorTeacherId,
        string? currentUserId,
        string? schoolId)
    {
        if (string.IsNullOrWhiteSpace(currentUserId) || string.IsNullOrWhiteSpace(schoolId))
            return false;

        var teacher = await _teacherRepository.GetByUserIdAsync(currentUserId);

        if (teacher is null)
            return false;

        if (teacher.SchoolId != schoolId)
            return false;

        if (!string.IsNullOrWhiteSpace(classroomAdvisorTeacherId) &&
            classroomAdvisorTeacherId == teacher.Id)
            return true;

        var teacherLessons = await _teacherLessonRepository.GetByTeacherIdAsync(teacher.Id);

        return teacherLessons.Any(x =>
            x.IsActive &&
            x.ClassroomId == classroomId &&
            x.SchoolId == schoolId
        );
    }

    private async Task<ClassroomListDto> MapToListDtoAsync(Classroom classroom)
    {
        string? teacherFullName = null;

        if (!string.IsNullOrWhiteSpace(classroom.TeacherId))
        {
            var teacher = await _teacherRepository.GetByIdAsync(classroom.TeacherId);

            if (teacher is not null)
            {
                var user = await _userRepository.GetByIdAsync(teacher.UserId);

                if (user is not null)
                    teacherFullName = $"{user.FirstName} {user.LastName}";
            }
        }

        var students = await _studentRepository.GetByClassroomIdAsync(classroom.Id);

        return new ClassroomListDto
        {
            Id = classroom.Id,
            SchoolId = classroom.SchoolId,
            Grade = classroom.Grade,
            Section = classroom.Section,
            TeacherId = classroom.TeacherId,
            TeacherFullName = teacherFullName,
            StudentCount = students.Count,
            IsActive = classroom.IsActive
        };
    }
}