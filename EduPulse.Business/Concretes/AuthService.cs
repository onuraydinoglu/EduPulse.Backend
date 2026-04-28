using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Auth;
using EduPulse.DTOs.Common;
using EduPulse.Entities.Users;
using EduPulse.Repository.Abstracts;

namespace EduPulse.Business.Concretes;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUserRepository userRepository,
        ISchoolRepository schoolRepository,
        IRoleRepository roleRepository,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _schoolRepository = schoolRepository;
        _roleRepository = roleRepository;
        _jwtService = jwtService;
    }

    public async Task<Result> RegisterUserAsync(RegisterUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName))
            return Result.Failure("Ad boş olamaz.", 400);

        if (string.IsNullOrWhiteSpace(dto.LastName))
            return Result.Failure("Soyad boş olamaz.", 400);

        if (string.IsNullOrWhiteSpace(dto.Email))
            return Result.Failure("Email boş olamaz.", 400);

        if (string.IsNullOrWhiteSpace(dto.Password))
            return Result.Failure("Şifre boş olamaz.", 400);

        if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
            return Result.Failure("Telefon numarası boş olamaz.", 400);

        if (string.IsNullOrWhiteSpace(dto.SchoolCode))
            return Result.Failure("Okul kodu boş olamaz.", 400);

        var existingUser = await _userRepository.GetByEmailAsync(dto.Email.Trim());

        if (existingUser is not null)
            return Result.Failure("Bu email zaten kullanılıyor.", 400);

        var school = await _schoolRepository.GetBySchoolCodeAsync(dto.SchoolCode.Trim());

        if (school is null)
            return Result.Failure("Okul kodu geçersiz.", 400);

        if (!school.IsActive)
            return Result.Failure("Bu okul aktif değil.", 400);

        var hasSchoolAdmin = await _userRepository.ExistsSchoolAdminAsync(school.Id);

        if (hasSchoolAdmin)
            return Result.Failure("Bu okul için zaten bir müdür hesabı oluşturulmuş.", 400);

        var role = await _roleRepository.GetByNameAsync("schooladmin");

        if (role is null)
            return Result.Failure("SchoolAdmin rolü bulunamadı.", 404);

        var user = new User
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),

            RoleId = role.Id,
            RoleName = role.Name,

            SchoolId = school.Id,
            IsActive = true
        };

        await _userRepository.CreateAsync(user);

        return Result.Success("Müdür hesabı başarıyla oluşturuldu.");
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);

        if (user is null)
            return Result<LoginResponseDto>.Failure("Email veya şifre hatalı.", 400);

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!isPasswordValid)
            return Result<LoginResponseDto>.Failure("Email veya şifre hatalı.", 400);

        if (!user.IsActive)
            return Result<LoginResponseDto>.Failure("Kullanıcı pasif.", 403);

        var token = _jwtService.CreateToken(user);

        var response = new LoginResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            RoleId = user.RoleId,
            RoleName = user.RoleName,
            SchoolId = user.SchoolId,
            Token = token
        };

        return Result<LoginResponseDto>.Success(response, "Giriş başarılı.");
    }
}