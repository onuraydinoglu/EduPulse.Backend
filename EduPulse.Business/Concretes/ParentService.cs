using EduPulse.Business.Abstracts;
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

    public async Task<List<ParentListDto>> GetAllAsync()
    {
        var parents = await _parentRepository.GetAllAsync();

        return parents.Select(x => new ParentListDto
        {
            Id = x.Id,
            FullName = $"{x.FirstName} {x.LastName}",
            PhoneNumber = x.PhoneNumber,
            Email = x.Email,
            SchoolId = x.SchoolId,
            IsActive = x.IsActive
        }).ToList();
    }

    public async Task<List<ParentListDto>> GetBySchoolIdAsync(string schoolId)
    {
        var parents = await _parentRepository.GetBySchoolIdAsync(schoolId);

        return parents.Select(x => new ParentListDto
        {
            Id = x.Id,
            FullName = $"{x.FirstName} {x.LastName}",
            PhoneNumber = x.PhoneNumber,
            Email = x.Email,
            SchoolId = x.SchoolId,
            IsActive = x.IsActive
        }).ToList();
    }

    public async Task<ParentListDto?> GetByIdAsync(string id)
    {
        var parent = await _parentRepository.GetByIdAsync(id);

        if (parent is null)
            return null;

        return new ParentListDto
        {
            Id = parent.Id,
            FullName = $"{parent.FirstName} {parent.LastName}",
            PhoneNumber = parent.PhoneNumber,
            Email = parent.Email,
            SchoolId = parent.SchoolId,
            IsActive = parent.IsActive
        };
    }

    public async Task CreateAsync(CreateParentDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            throw new ArgumentException(validationResult.Errors.First().ErrorMessage);

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            throw new ArgumentException("Okul bulunamadı.");

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
    }

    public async Task UpdateAsync(UpdateParentDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            throw new ArgumentException(validationResult.Errors.First().ErrorMessage);

        var parent = await _parentRepository.GetByIdAsync(dto.Id);

        if (parent is null)
            throw new ArgumentException("Veli bulunamadı.");

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            throw new ArgumentException("Okul bulunamadı.");

        parent.FirstName = dto.FirstName;
        parent.LastName = dto.LastName;
        parent.PhoneNumber = dto.PhoneNumber;
        parent.Email = dto.Email;
        parent.SchoolId = dto.SchoolId;
        parent.IsActive = dto.IsActive;

        await _parentRepository.UpdateAsync(parent);
    }

    public async Task DeleteAsync(string id)
    {
        var parent = await _parentRepository.GetByIdAsync(id);

        if (parent is null)
            throw new ArgumentException("Veli bulunamadı.");

        await _parentRepository.DeleteAsync(id);
    }
}