using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Teachers;
using EduPulse.Entities.Teachers;
using EduPulse.Repository.Abstracts;
using FluentValidation;

namespace EduPulse.Business.Concretes;

public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly IValidator<CreateTeacherDto> _createValidator;
    private readonly IValidator<UpdateTeacherDto> _updateValidator;

    public TeacherService(
        ITeacherRepository teacherRepository,
        ISchoolRepository schoolRepository,
        IValidator<CreateTeacherDto> createValidator,
        IValidator<UpdateTeacherDto> updateValidator)
    {
        _teacherRepository = teacherRepository;
        _schoolRepository = schoolRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<List<TeacherListDto>> GetAllAsync()
    {
        var teachers = await _teacherRepository.GetAllAsync();

        return teachers.Select(x => new TeacherListDto
        {
            Id = x.Id,
            FullName = $"{x.FirstName} {x.LastName}",
            PhoneNumber = x.PhoneNumber,
            Email = x.Email,
            SchoolId = x.SchoolId,
            IsActive = x.IsActive
        }).ToList();
    }

    public async Task<List<TeacherListDto>> GetBySchoolIdAsync(string schoolId)
    {
        var teachers = await _teacherRepository.GetBySchoolIdAsync(schoolId);

        return teachers.Select(x => new TeacherListDto
        {
            Id = x.Id,
            FullName = $"{x.FirstName} {x.LastName}",
            PhoneNumber = x.PhoneNumber,
            Email = x.Email,
            SchoolId = x.SchoolId,
            IsActive = x.IsActive
        }).ToList();
    }

    public async Task<TeacherListDto?> GetByIdAsync(string id)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);

        if (teacher is null)
            return null;

        return new TeacherListDto
        {
            Id = teacher.Id,
            FullName = $"{teacher.FirstName} {teacher.LastName}",
            PhoneNumber = teacher.PhoneNumber,
            Email = teacher.Email,
            SchoolId = teacher.SchoolId,
            IsActive = teacher.IsActive
        };
    }

    public async Task CreateAsync(CreateTeacherDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            throw new ArgumentException(validationResult.Errors.First().ErrorMessage);

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            throw new ArgumentException("Okul bulunamadı.");

        var teacher = new Teacher
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            SchoolId = dto.SchoolId,
            IsActive = true
        };

        await _teacherRepository.CreateAsync(teacher);
    }

    public async Task UpdateAsync(UpdateTeacherDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            throw new ArgumentException(validationResult.Errors.First().ErrorMessage);

        var teacher = await _teacherRepository.GetByIdAsync(dto.Id);

        if (teacher is null)
            throw new ArgumentException("Öğretmen bulunamadı.");

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            throw new ArgumentException("Okul bulunamadı.");

        teacher.FirstName = dto.FirstName;
        teacher.LastName = dto.LastName;
        teacher.PhoneNumber = dto.PhoneNumber;
        teacher.Email = dto.Email;
        teacher.SchoolId = dto.SchoolId;
        teacher.IsActive = dto.IsActive;

        await _teacherRepository.UpdateAsync(teacher);
    }

    public async Task DeleteAsync(string id)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);

        if (teacher is null)
            throw new ArgumentException("Öğretmen bulunamadı.");

        await _teacherRepository.DeleteAsync(id);
    }
}