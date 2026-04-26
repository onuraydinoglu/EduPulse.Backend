using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Schools;
using EduPulse.Entities.Schools;
using EduPulse.Repository.Abstracts;
using FluentValidation;

namespace EduPulse.Business.Concretes;

public class SchoolService : ISchoolService
{
    private readonly ISchoolRepository _schoolRepository;
    private readonly IValidator<CreateSchoolDto> _createValidator;
    private readonly IValidator<UpdateSchoolDto> _updateValidator;

    public SchoolService(
        ISchoolRepository schoolRepository,
        IValidator<CreateSchoolDto> createValidator,
        IValidator<UpdateSchoolDto> updateValidator)
    {
        _schoolRepository = schoolRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<List<SchoolListDto>> GetAllAsync()
    {
        var schools = await _schoolRepository.GetAllAsync();

        return schools.Select(x => new SchoolListDto
        {
            Id = x.Id,
            Name = x.Name,
            City = x.City,
            District = x.District,
            PhoneNumber = x.PhoneNumber,
            Email = x.Email,
            PrincipalName = x.PrincipalName,
            IsActive = x.IsActive
        }).ToList();
    }

    public async Task<SchoolListDto?> GetByIdAsync(string id)
    {
        var school = await _schoolRepository.GetByIdAsync(id);

        if (school is null)
            return null;

        return new SchoolListDto
        {
            Id = school.Id,
            Name = school.Name,
            City = school.City,
            District = school.District,
            PhoneNumber = school.PhoneNumber,
            Email = school.Email,
            PrincipalName = school.PrincipalName,
            IsActive = school.IsActive
        };
    }

    public async Task CreateAsync(CreateSchoolDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            throw new ArgumentException(validationResult.Errors.First().ErrorMessage);

        var school = new School
        {
            Name = dto.Name,
            City = dto.City,
            District = dto.District,
            Address = dto.Address,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            PrincipalName = dto.PrincipalName,
            IsActive = true
        };

        await _schoolRepository.CreateAsync(school);
    }

    public async Task UpdateAsync(UpdateSchoolDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            throw new ArgumentException(validationResult.Errors.First().ErrorMessage);

        var school = await _schoolRepository.GetByIdAsync(dto.Id);

        if (school is null)
            throw new ArgumentException("Okul bulunamadı.");

        school.Name = dto.Name;
        school.City = dto.City;
        school.District = dto.District;
        school.Address = dto.Address;
        school.PhoneNumber = dto.PhoneNumber;
        school.Email = dto.Email;
        school.PrincipalName = dto.PrincipalName;
        school.IsActive = dto.IsActive;

        await _schoolRepository.UpdateAsync(school);
    }

    public async Task DeleteAsync(string id)
    {
        var school = await _schoolRepository.GetByIdAsync(id);

        if (school is null)
            throw new ArgumentException("Okul bulunamadı.");

        await _schoolRepository.DeleteAsync(id);
    }
}