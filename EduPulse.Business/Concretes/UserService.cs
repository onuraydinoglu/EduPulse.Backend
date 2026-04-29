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

    private static UserListDto MapToListDto(User user)
    {
        return new UserListDto
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
    }

    public async Task<Result<List<UserListDto>>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        var result = users.Select(MapToListDto).ToList();

        return Result<List<UserListDto>>.Success(result);
    }

    public async Task<Result<UserListDto>> GetByIdAsync(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return Result<UserListDto>.Failure("Kullanıcı bulunamadı.", 404);

        var result = MapToListDto(user);

        return Result<UserListDto>.Success(result);
    }

    public async Task<Result<List<UserListDto>>> GetBySchoolIdAsync(string schoolId)
    {
        var users = await _userRepository.GetBySchoolIdAsync(schoolId);

        var result = users.Select(MapToListDto).ToList();

        return Result<List<UserListDto>>.Success(result);
    }

    public async Task<Result<List<UserListDto>>> GetTeachersAsync(string? schoolId)
    {
        var users = string.IsNullOrWhiteSpace(schoolId)
            ? await _userRepository.GetByRoleNameAsync("teacher")
            : await _userRepository.GetBySchoolIdAndRoleNameAsync(schoolId, "teacher");

        var result = users.Select(MapToListDto).ToList();

        return Result<List<UserListDto>>.Success(result);
    }

    public async Task<Result<List<UserListDto>>> GetOfficersAsync(string? schoolId)
    {
        var users = string.IsNullOrWhiteSpace(schoolId)
            ? await _userRepository.GetByRoleNameAsync("officer")
            : await _userRepository.GetBySchoolIdAndRoleNameAsync(schoolId, "officer");

        var result = users.Select(MapToListDto).ToList();

        return Result<List<UserListDto>>.Success(result);
    }

    public async Task<Result<List<UserListDto>>> GetStudentsAsync(string? schoolId)
    {
        var users = string.IsNullOrWhiteSpace(schoolId)
            ? await _userRepository.GetByRoleNameAsync("student")
            : await _userRepository.GetBySchoolIdAndRoleNameAsync(schoolId, "student");

        var result = users.Select(MapToListDto).ToList();

        return Result<List<UserListDto>>.Success(result);
    }

    public async Task<Result<List<UserListDto>>> GetAllForCurrentUserAsync(
        string? currentRoleName,
        string? currentSchoolId)
    {
        currentRoleName = currentRoleName?.Trim().ToLower();

        if (currentRoleName == "superadmin")
            return await GetAllAsync();

        if (currentRoleName == "schooladmin")
        {
            if (string.IsNullOrWhiteSpace(currentSchoolId))
                return Result<List<UserListDto>>.Failure("Okul bilgisi bulunamadı.", 403);

            return await GetBySchoolIdAsync(currentSchoolId);
        }

        return Result<List<UserListDto>>.Failure("Bu işlem için yetkiniz yok.", 403);
    }

    public async Task<Result<UserListDto>> GetByIdForCurrentUserAsync(
        string id,
        string? currentRoleName,
        string? currentSchoolId)
    {
        currentRoleName = currentRoleName?.Trim().ToLower();

        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return Result<UserListDto>.Failure("Kullanıcı bulunamadı.", 404);

        if (currentRoleName == "superadmin")
            return Result<UserListDto>.Success(MapToListDto(user));

        if (currentRoleName == "schooladmin")
        {
            if (string.IsNullOrWhiteSpace(currentSchoolId))
                return Result<UserListDto>.Failure("Okul bilgisi bulunamadı.", 403);

            if (user.SchoolId != currentSchoolId)
                return Result<UserListDto>.Failure("Bu kullanıcıya erişim yetkiniz yok.", 403);

            return Result<UserListDto>.Success(MapToListDto(user));
        }

        return Result<UserListDto>.Failure("Bu işlem için yetkiniz yok.", 403);
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

    public async Task<Result> UpdateForCurrentUserAsync(
        UpdateUserDto dto,
        string? currentRoleName,
        string? currentSchoolId)
    {
        currentRoleName = currentRoleName?.Trim().ToLower();

        var user = await _userRepository.GetByIdAsync(dto.Id);

        if (user is null)
            return Result.Failure("Kullanıcı bulunamadı.", 404);

        if (currentRoleName == "schooladmin")
        {
            if (string.IsNullOrWhiteSpace(currentSchoolId))
                return Result.Failure("Okul bilgisi bulunamadı.", 403);

            if (user.SchoolId != currentSchoolId)
                return Result.Failure("Bu kullanıcıyı güncelleme yetkiniz yok.", 403);
        }
        else if (currentRoleName != "superadmin")
        {
            return Result.Failure("Bu işlem için yetkiniz yok.", 403);
        }

        return await UpdateAsync(dto);
    }

    public async Task<Result> DeleteForCurrentUserAsync(
        string id,
        string? currentRoleName,
        string? currentSchoolId)
    {
        currentRoleName = currentRoleName?.Trim().ToLower();

        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return Result.Failure("Kullanıcı bulunamadı.", 404);

        if (currentRoleName == "schooladmin")
        {
            if (string.IsNullOrWhiteSpace(currentSchoolId))
                return Result.Failure("Okul bilgisi bulunamadı.", 403);

            if (user.SchoolId != currentSchoolId)
                return Result.Failure("Bu kullanıcıyı silme yetkiniz yok.", 403);
        }
        else if (currentRoleName != "superadmin")
        {
            return Result.Failure("Bu işlem için yetkiniz yok.", 403);
        }

        return await DeleteAsync(id);
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