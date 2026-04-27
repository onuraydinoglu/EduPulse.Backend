using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Common;
using EduPulse.DTOs.Roles;
using EduPulse.Entities.Roles;
using EduPulse.Repository.Abstracts;
using FluentValidation;

namespace EduPulse.Business.Concretes;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IValidator<CreateRoleDto> _createValidator;
    private readonly IValidator<UpdateRoleDto> _updateValidator;

    public RoleService(
        IRoleRepository roleRepository,
        IValidator<CreateRoleDto> createValidator,
        IValidator<UpdateRoleDto> updateValidator)
    {
        _roleRepository = roleRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<List<RoleListDto>>> GetAllAsync()
    {
        var roles = await _roleRepository.GetAllAsync();

        var result = roles.Select(x => new RoleListDto
        {
            Id = x.Id,
            Name = x.Name,
            IsActive = x.IsActive
        }).ToList();

        return Result<List<RoleListDto>>.Success(result);
    }

    public async Task<Result<RoleListDto>> GetByIdAsync(string id)
    {
        var role = await _roleRepository.GetByIdAsync(id);

        if (role is null)
            return Result<RoleListDto>.Failure("Rol bulunamadı.", 404);

        var result = new RoleListDto
        {
            Id = role.Id,
            Name = role.Name,
            IsActive = role.IsActive
        };

        return Result<RoleListDto>.Success(result);
    }
    public async Task<Result> CreateAsync(CreateRoleDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var existingRole = await _roleRepository.GetByNameAsync(dto.Name);

        if (existingRole is not null)
            return Result.Failure("Bu rol zaten kayıtlı.", 400);

        var role = new Role
        {
            Name = dto.Name,
            IsActive = true
        };

        await _roleRepository.CreateAsync(role);

        return Result.Success("Rol başarıyla oluşturuldu.");
    }

    public async Task<Result> UpdateAsync(UpdateRoleDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var role = await _roleRepository.GetByIdAsync(dto.Id);

        if (role is null)
            return Result.Failure("Rol bulunamadı.", 404);

        var existingRole = await _roleRepository.GetByNameAsync(dto.Name);

        if (existingRole is not null && existingRole.Id != dto.Id)
            return Result.Failure("Bu rol adı başka bir kayıt tarafından kullanılıyor.", 400);

        role.Name = dto.Name;
        role.IsActive = dto.IsActive;

        await _roleRepository.UpdateAsync(role);

        return Result.Success("Rol başarıyla güncellendi.");
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var role = await _roleRepository.GetByIdAsync(id);

        if (role is null)
            return Result.Failure("Rol bulunamadı.", 404);

        await _roleRepository.DeleteAsync(id);

        return Result.Success("Rol başarıyla silindi.");
    }
}