using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Auth;
using EduPulse.DTOs.Common;
using EduPulse.Entities.Schools;
using EduPulse.Entities.Users;
using EduPulse.Repository.Abstracts;
using FluentValidation;

namespace EduPulse.Business.Concretes;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IValidator<RegisterSchoolDto> _registerSchoolValidator;
    private readonly IValidator<LoginDto> _loginValidator;

    public AuthService(
        IUserRepository userRepository,
        ISchoolRepository schoolRepository,
        IRoleRepository roleRepository,
        IValidator<RegisterSchoolDto> registerSchoolValidator,
        IValidator<LoginDto> loginValidator)
    {
        _userRepository = userRepository;
        _schoolRepository = schoolRepository;
        _roleRepository = roleRepository;
        _registerSchoolValidator = registerSchoolValidator;
        _loginValidator = loginValidator;
    }

    public async Task<Result> RegisterSchoolAsync(RegisterSchoolDto dto)
    {
        var validationResult = await _registerSchoolValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var existingUser = await _userRepository.GetByEmailAsync(dto.AdminEmail);

        if (existingUser is not null)
            return Result.Failure("Bu e-posta zaten kullanılıyor.", 400);

        var existingSchool = await _schoolRepository.GetByEmailAsync(dto.SchoolEmail);

        if (existingSchool is not null)
            return Result.Failure("Bu okul e-posta adresi zaten kullanılıyor.", 400);

        var school = new School
        {
            Name = dto.SchoolName,
            City = dto.City,
            District = dto.District,
            Address = dto.Address,
            PhoneNumber = dto.SchoolPhoneNumber,
            Email = dto.SchoolEmail,
            PrincipalName = dto.AdminFullName,
            IsActive = true
        };

        await _schoolRepository.CreateAsync(school);

        var role = await _roleRepository.GetByNameAsync("SchoolAdmin");

        if (role is null)
            return Result.Failure("SchoolAdmin rolü bulunamadı.", 500);

        var user = new User
        {
            FullName = dto.AdminFullName,
            Email = dto.AdminEmail,
            PasswordHash = HashPassword(dto.AdminPassword),

            RoleId = role.Id,
            RoleName = role.Name,

            SchoolId = school.Id,
            IsActive = true
        };

        await _userRepository.CreateAsync(user);

        return Result.Success("Okul ve yönetici hesabı oluşturuldu.");
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginDto dto)
    {
        var validationResult = await _loginValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result<LoginResponseDto>.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var user = await _userRepository.GetByEmailAsync(dto.Email);

        if (user is null)
            return Result<LoginResponseDto>.Failure("Kullanıcı bulunamadı.", 404);

        if (!VerifyPassword(dto.Password, user.PasswordHash))
            return Result<LoginResponseDto>.Failure("Şifre hatalı.", 400);

        var response = new LoginResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            RoleId = user.RoleId,
            RoleName = user.RoleName,
            SchoolId = user.SchoolId,
            TeacherId = user.TeacherId,
            StudentId = user.StudentId,
            ParentId = user.ParentId
        };

        return Result<LoginResponseDto>.Success(response);
    }

    // Basit hash (sonra iyileştiririz)
    private string HashPassword(string password)
    {
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }

    private bool VerifyPassword(string password, string hash)
    {
        var hashed = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        return hashed == hash;
    }
}