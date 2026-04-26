using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Common;
using EduPulse.DTOs.Parents;
using EduPulse.Entities.Parents;
using EduPulse.Repository.Abstracts;
using FluentValidation;

namespace EduPulse.Business.Concretes;

public class ParentService : IParentService
{
    private readonly IParentRepository _parentRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly IValidator<CreateParentDto> _createValidator;
    private readonly IValidator<UpdateParentDto> _updateValidator;

    public ParentService(
        IParentRepository parentRepository,
        ISchoolRepository schoolRepository,
        IValidator<CreateParentDto> createValidator,
        IValidator<UpdateParentDto> updateValidator)
    {
        _parentRepository = parentRepository;
        _schoolRepository = schoolRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<List<ParentListDto>>> GetAllAsync()
    {
        var parents = await _parentRepository.GetAllAsync();

        var result = parents.Select(x => new ParentListDto
        {
            Id = x.Id,
            FullName = $"{x.FirstName} {x.LastName}",
            PhoneNumber = x.PhoneNumber,
            Email = x.Email,
            SchoolId = x.SchoolId,
            IsActive = x.IsActive
        }).ToList();

        return Result<List<ParentListDto>>.Success(result, "Veliler başarıyla listelendi.");
    }

    public async Task<Result<List<ParentListDto>>> GetBySchoolIdAsync(string schoolId)
    {
        var school = await _schoolRepository.GetByIdAsync(schoolId);

        if (school is null)
            return Result<List<ParentListDto>>.Failure("Okul bulunamadı.", 404);

        var parents = await _parentRepository.GetBySchoolIdAsync(schoolId);

        var result = parents.Select(x => new ParentListDto
        {
            Id = x.Id,
            FullName = $"{x.FirstName} {x.LastName}",
            PhoneNumber = x.PhoneNumber,
            Email = x.Email,
            SchoolId = x.SchoolId,
            IsActive = x.IsActive
        }).ToList();

        return Result<List<ParentListDto>>.Success(result, "Okula ait veliler başarıyla listelendi.");
    }

    public async Task<Result<ParentListDto>> GetByIdAsync(string id)
    {
        var parent = await _parentRepository.GetByIdAsync(id);

        if (parent is null)
            return Result<ParentListDto>.Failure("Veli bulunamadı.", 404);

        var result = new ParentListDto
        {
            Id = parent.Id,
            FullName = $"{parent.FirstName} {parent.LastName}",
            PhoneNumber = parent.PhoneNumber,
            Email = parent.Email,
            SchoolId = parent.SchoolId,
            IsActive = parent.IsActive
        };

        return Result<ParentListDto>.Success(result, "Veli başarıyla getirildi.");
    }

    public async Task<Result> CreateAsync(CreateParentDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            return Result.Failure("Okul bulunamadı.", 404);

        var parent = new Parent
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            SchoolId = dto.SchoolId,
            IsActive = true
        };

        await _parentRepository.CreateAsync(parent);

        return Result.Success("Veli başarıyla oluşturuldu.", 201);
    }

    public async Task<Result> UpdateAsync(UpdateParentDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var parent = await _parentRepository.GetByIdAsync(dto.Id);

        if (parent is null)
            return Result.Failure("Veli bulunamadı.", 404);

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            return Result.Failure("Okul bulunamadı.", 404);

        parent.FirstName = dto.FirstName;
        parent.LastName = dto.LastName;
        parent.PhoneNumber = dto.PhoneNumber;
        parent.Email = dto.Email;
        parent.SchoolId = dto.SchoolId;
        parent.IsActive = dto.IsActive;

        await _parentRepository.UpdateAsync(parent);

        return Result.Success("Veli başarıyla güncellendi.");
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var parent = await _parentRepository.GetByIdAsync(id);

        if (parent is null)
            return Result.Failure("Veli bulunamadı.", 404);

        await _parentRepository.DeleteAsync(id);

        return Result.Success("Veli başarıyla silindi.");
    }
}