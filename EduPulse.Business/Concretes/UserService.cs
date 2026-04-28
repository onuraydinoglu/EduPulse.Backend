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

    public async Task<Result> CreateAsync(CreateUserDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var existingUser = await _userRepository.GetByEmailAsync(dto.Email);

        if (existingUser is not null)
            return Result.Failure("Bu email adresi zaten kullanılıyor.", 400);

        var role = await _roleRepository.GetByIdAsync(dto.RoleId);

        if (role is null)
            return Result.Failure("Rol bulunamadı.", 404);

        if (!string.IsNullOrWhiteSpace(dto.SchoolId))
        {
            var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

            if (school is null)
                return Result.Failure("Okul bulunamadı.", 404);
        }

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            RoleId = role.Id,
            RoleName = role.Name,
            SchoolId = dto.SchoolId,
            IsActive = true
        };

        await _userRepository.CreateAsync(user);

        return Result.Success("Kullanıcı başarıyla oluşturuldu.");
    }

    public async Task<Result> UpdateAsync(UpdateUserDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var user = await _userRepository.GetByIdAsync(dto.Id);

        if (user is null)
            return Result.Failure("Kullanıcı bulunamadı.", 404);

        var role = await _roleRepository.GetByIdAsync(dto.RoleId);

        if (role is null)
            return Result.Failure("Rol bulunamadı.", 404);

        if (!string.IsNullOrWhiteSpace(dto.SchoolId))
        {
            var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

            if (school is null)
                return Result.Failure("Okul bulunamadı.", 404);
        }

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;
        user.PhoneNumber = dto.PhoneNumber;
        user.RoleId = role.Id;
        user.RoleName = role.Name;
        user.SchoolId = dto.SchoolId;
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