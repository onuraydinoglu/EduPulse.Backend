using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Common;
using EduPulse.DTOs.Users;
using EduPulse.Entities.Users;
using EduPulse.Repository.Abstracts;
using FluentValidation;

namespace EduPulse.Business.Concretes;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly IValidator<CreateUserDto> _createValidator;
    private readonly IValidator<UpdateUserDto> _updateValidator;

    public UserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ISchoolRepository schoolRepository,
        IValidator<CreateUserDto> createValidator,
        IValidator<UpdateUserDto> updateValidator)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _schoolRepository = schoolRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    private static string GenerateTemporaryPassword(string firstName)
    {
        var normalizedFirstName = firstName
            .Trim()
            .ToLower()
            .Replace("ı", "i")
            .Replace("ğ", "g")
            .Replace("ü", "u")
            .Replace("ş", "s")
            .Replace("ö", "o")
            .Replace("ç", "c");

        var randomNumber = Random.Shared.Next(1000, 10000);

        return $"{normalizedFirstName}{randomNumber}";
    }

    public async Task<Result<List<UserListDto>>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        var result = users.Select(x => new UserListDto
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            PhoneNumber = x.PhoneNumber,
            RoleId = x.RoleId,
            RoleName = x.RoleName,
            SchoolId = x.SchoolId,
            IsActive = x.IsActive
        }).ToList();

        return Result<List<UserListDto>>.Success(result);
    }

    public async Task<Result<UserListDto>> GetByIdAsync(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return Result<UserListDto>.Failure("Kullanıcı bulunamadı.", 404);

        var result = new UserListDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            RoleId = user.RoleId,
            RoleName = user.RoleName,
            SchoolId = user.SchoolId,
            IsActive = user.IsActive
        };

        return Result<UserListDto>.Success(result);
    }

    public async Task<Result<List<UserListDto>>> GetBySchoolIdAsync(string schoolId)
    {
        var users = await _userRepository.GetBySchoolIdAsync(schoolId);

        var result = users.Select(x => new UserListDto
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            PhoneNumber = x.PhoneNumber,
            RoleId = x.RoleId,
            RoleName = x.RoleName,
            SchoolId = x.SchoolId,
            IsActive = x.IsActive
        }).ToList();

        return Result<List<UserListDto>>.Success(result);
    }

    public async Task<Result> CreateUserAsync(CreateUserDto dto, string? schoolId, string roleName)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 403);

        if (string.IsNullOrWhiteSpace(roleName))
            return Result.Failure("Rol bilgisi bulunamadı.", 400);

        var requestedRoleName = roleName.Trim().ToLower();

        var forbiddenRoles = new[] { "superadmin", "schooladmin" };

        if (forbiddenRoles.Contains(requestedRoleName))
            return Result.Failure("Bu rol atanamaz.", 400);

        var school = await _schoolRepository.GetByIdAsync(schoolId);

        if (school is null)
            return Result.Failure("Okul bulunamadı.", 404);

        if (!school.IsActive)
            return Result.Failure("Okul aktif değil.", 400);

        var existingUser = await _userRepository.GetByEmailAsync(dto.Email.Trim());

        if (existingUser is not null)
            return Result.Failure("Bu email adresi zaten kullanılıyor.", 400);

        var role = await _roleRepository.GetByNameAsync(requestedRoleName);

        if (role is null)
            return Result.Failure("Rol bulunamadı.", 404);

        var temporaryPassword = GenerateTemporaryPassword(dto.FirstName);

        var user = new User
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword),
            RoleId = role.Id,
            RoleName = role.Name,
            SchoolId = school.Id,
            IsActive = true
        };

        await _userRepository.CreateAsync(user);

        return Result.Success($"Kullanıcı başarıyla oluşturuldu. Geçici şifre: {temporaryPassword}");
    }

    public async Task<Result> UpdateAsync(UpdateUserDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var user = await _userRepository.GetByIdAsync(dto.Id);

        if (user is null)
            return Result.Failure("Kullanıcı bulunamadı.", 404);

        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();
        user.Email = dto.Email.Trim();
        user.PhoneNumber = dto.PhoneNumber.Trim();
        user.IsActive = dto.IsActive;

        await _userRepository.UpdateAsync(user);

        return Result.Success("Kullanıcı başarıyla güncellendi.");
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return Result.Failure("Kullanıcı bulunamadı.", 404);

        await _userRepository.DeleteAsync(id);

        return Result.Success("Kullanıcı başarıyla silindi.");
    }
}