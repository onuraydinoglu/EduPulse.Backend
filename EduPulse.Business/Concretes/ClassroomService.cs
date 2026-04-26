using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Classrooms;
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

    public async Task<List<ClassroomListDto>> GetAllAsync()
    {
        var classrooms = await _classroomRepository.GetAllAsync();
        var teachers = await _teacherRepository.GetAllAsync();

        return classrooms.Select(x =>
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
    }

    public async Task<List<ClassroomListDto>> GetBySchoolIdAsync(string schoolId)
    {
        var classrooms = await _classroomRepository.GetBySchoolIdAsync(schoolId);
        var teachers = await _teacherRepository.GetAllAsync();

        return classrooms.Select(x =>
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
    }

    public async Task<ClassroomListDto?> GetByIdAsync(string id)
    {
        var classroom = await _classroomRepository.GetByIdAsync(id);

        if (classroom is null)
            return null;

        var teacher = classroom.TeacherId != null
            ? await _teacherRepository.GetByIdAsync(classroom.TeacherId)
            : null;

        return new ClassroomListDto
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
    }

    public async Task CreateAsync(CreateClassroomDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            throw new ArgumentException(validationResult.Errors.First().ErrorMessage);

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);
        if (school is null)
            throw new ArgumentException("Okul bulunamadı.");

        if (!string.IsNullOrWhiteSpace(dto.TeacherId))
        {
            var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);
            if (teacher is null)
                throw new ArgumentException("Öğretmen bulunamadı.");
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
    }

    public async Task UpdateAsync(UpdateClassroomDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            throw new ArgumentException(validationResult.Errors.First().ErrorMessage);

        var classroom = await _classroomRepository.GetByIdAsync(dto.Id);
        if (classroom is null)
            throw new ArgumentException("Sınıf bulunamadı.");

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);
        if (school is null)
            throw new ArgumentException("Okul bulunamadı.");

        if (!string.IsNullOrWhiteSpace(dto.TeacherId))
        {
            var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);
            if (teacher is null)
                throw new ArgumentException("Öğretmen bulunamadı.");
        }

        classroom.SchoolId = dto.SchoolId;
        classroom.Grade = dto.Grade;
        classroom.Section = dto.Section;
        classroom.TeacherId = dto.TeacherId;
        classroom.IsActive = dto.IsActive;

        await _classroomRepository.UpdateAsync(classroom);
    }

    public async Task DeleteAsync(string id)
    {
        var classroom = await _classroomRepository.GetByIdAsync(id);

        if (classroom is null)
            throw new ArgumentException("Sınıf bulunamadı.");

        await _classroomRepository.DeleteAsync(id);
    }
}