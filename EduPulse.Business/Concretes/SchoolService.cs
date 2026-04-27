using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Common;
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

    public async Task<Result<List<SchoolListDto>>> GetAllAsync()
    {
        var schools = await _schoolRepository.GetAllAsync();

        var result = schools.Select(x => new SchoolListDto
        {
            Id = x.Id,
            Name = x.Name,
            City = x.City,
            District = x.District,
            Address = x.Address,
            PhoneNumber = x.PhoneNumber,
            Email = x.Email,
            PrincipalName = x.PrincipalName,
            IsActive = x.IsActive
        }).ToList();

        return Result<List<SchoolListDto>>.Success(result, "Okullar başarıyla listelendi.");
    }

    public async Task<Result<SchoolListDto>> GetByIdAsync(string id)
    {
        var school = await _schoolRepository.GetByIdAsync(id);

        if (school is null)
            return Result<SchoolListDto>.Failure("Okul bulunamadı.", 404);

        var result = new SchoolListDto
        {
            Id = school.Id,
            Name = school.Name,
            City = school.City,
            District = school.District,
            Address = school.Address,
            PhoneNumber = school.PhoneNumber,
            Email = school.Email,
            PrincipalName = school.PrincipalName,
            IsActive = school.IsActive
        };

        return Result<SchoolListDto>.Success(result, "Okul başarıyla getirildi.");
    }

    public async Task<Result> CreateAsync(CreateSchoolDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

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

        return Result.Success("Okul başarıyla oluşturuldu.", 201);
    }

    public async Task<Result> UpdateAsync(UpdateSchoolDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var school = await _schoolRepository.GetByIdAsync(dto.Id);

        if (school is null)
            return Result.Failure("Okul bulunamadı.", 404);

        school.Name = dto.Name;
        school.City = dto.City;
        school.District = dto.District;
        school.Address = dto.Address;
        school.PhoneNumber = dto.PhoneNumber;
        school.Email = dto.Email;
        school.PrincipalName = dto.PrincipalName;
        school.IsActive = dto.IsActive;

        await _schoolRepository.UpdateAsync(school);

        return Result.Success("Okul başarıyla güncellendi.");
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var school = await _schoolRepository.GetByIdAsync(id);

        if (school is null)
            return Result.Failure("Okul bulunamadı.", 404);

        await _schoolRepository.DeleteAsync(id);

        return Result.Success("Okul başarıyla silindi.");
    }
}