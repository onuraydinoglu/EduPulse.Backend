using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Common;
using EduPulse.DTOs.Teachers;
using EduPulse.Entities.Teachers;
using EduPulse.Entities.Users;
using EduPulse.Repository.Abstracts;
using FluentValidation;

namespace EduPulse.Business.Concretes;

public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IValidator<CreateTeacherDto> _createValidator;
    private readonly IValidator<UpdateTeacherDto> _updateValidator;

    public TeacherService(
        ITeacherRepository teacherRepository,
        ISchoolRepository schoolRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IValidator<CreateTeacherDto> createValidator,
        IValidator<UpdateTeacherDto> updateValidator)
    {
        _teacherRepository = teacherRepository;
        _schoolRepository = schoolRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<List<TeacherListDto>>> GetAllAsync()
    {
        var teachers = await _teacherRepository.GetAllAsync();

        var result = teachers.Select(x => new TeacherListDto
        {
            Id = x.Id,
            FullName = $"{x.FirstName} {x.LastName}",
            PhoneNumber = x.PhoneNumber,
            Email = x.Email,
            SchoolId = x.SchoolId,
            IsActive = x.IsActive
        }).ToList();

        return Result<List<TeacherListDto>>.Success(result, "Öğretmenler başarıyla listelendi.");
    }

    public async Task<Result<List<TeacherListDto>>> GetBySchoolIdAsync(string schoolId)
    {
        var school = await _schoolRepository.GetByIdAsync(schoolId);

        if (school is null)
            return Result<List<TeacherListDto>>.Failure("Okul bulunamadı.", 404);

        var teachers = await _teacherRepository.GetBySchoolIdAsync(schoolId);

        var result = teachers.Select(x => new TeacherListDto
        {
            Id = x.Id,
            FullName = $"{x.FirstName} {x.LastName}",
            PhoneNumber = x.PhoneNumber,
            Email = x.Email,
            SchoolId = x.SchoolId,
            IsActive = x.IsActive
        }).ToList();

        return Result<List<TeacherListDto>>.Success(result, "Okula ait öğretmenler başarıyla listelendi.");
    }

    public async Task<Result<TeacherListDto>> GetByIdAsync(string id)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);

        if (teacher is null)
            return Result<TeacherListDto>.Failure("Öğretmen bulunamadı.", 404);

        var result = new TeacherListDto
        {
            Id = teacher.Id,
            FullName = $"{teacher.FirstName} {teacher.LastName}",
            PhoneNumber = teacher.PhoneNumber,
            Email = teacher.Email,
            SchoolId = teacher.SchoolId,
            IsActive = teacher.IsActive
        };

        return Result<TeacherListDto>.Success(result, "Öğretmen başarıyla getirildi.");
    }

    public async Task<Result> CreateAsync(CreateTeacherDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            return Result.Failure("Okul bulunamadı.", 404);

        var existingUser = await _userRepository.GetByEmailAsync(dto.Email);

        if (existingUser is not null)
            return Result.Failure("Bu e-posta adresi kullanıcı hesabı olarak zaten kayıtlı.", 400);

        var role = await _roleRepository.GetByNameAsync("Teacher");

        if (role is null)
            return Result.Failure("Teacher rolü bulunamadı.", 500);

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

        var user = new User
        {
            FirstName = teacher.FirstName,
            LastName = teacher.LastName,
            Email = teacher.Email,
            PasswordHash = HashPassword(dto.Password),
            RoleId = role.Id,
            RoleName = role.Name,
            SchoolId = teacher.SchoolId,
            IsActive = true
        };

        await _userRepository.CreateAsync(user);

        return Result.Success("Öğretmen ve kullanıcı hesabı başarıyla oluşturuldu.", 201);
    }

    public async Task<Result> UpdateAsync(UpdateTeacherDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var teacher = await _teacherRepository.GetByIdAsync(dto.Id);

        if (teacher is null)
            return Result.Failure("Öğretmen bulunamadı.", 404);

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            return Result.Failure("Okul bulunamadı.", 404);

        teacher.FirstName = dto.FirstName;
        teacher.LastName = dto.LastName;
        teacher.PhoneNumber = dto.PhoneNumber;
        teacher.Email = dto.Email;
        teacher.SchoolId = dto.SchoolId;
        teacher.IsActive = dto.IsActive;

        await _teacherRepository.UpdateAsync(teacher);

        return Result.Success("Öğretmen başarıyla güncellendi.");
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);

        if (teacher is null)
            return Result.Failure("Öğretmen bulunamadı.", 404);

        var user = await _userRepository.GetByEmailAsync(teacher.Email);

        if (user is not null)
            await _userRepository.DeleteAsync(user.Id);

        await _teacherRepository.DeleteAsync(id);

        return Result.Success("Öğretmen ve kullanıcı hesabı silindi.");
    }

    private string HashPassword(string password)
    {
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }
}