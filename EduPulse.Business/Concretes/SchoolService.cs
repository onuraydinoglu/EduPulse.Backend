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
            Email = x.Email,
            PhoneNumber = x.PhoneNumber,
            SchoolCode = x.SchoolCode,
            IsActive = x.IsActive
        }).ToList();

        return Result<List<SchoolListDto>>.Success(result, "Okullar listelendi.");
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
            Email = school.Email,
            PhoneNumber = school.PhoneNumber,
            SchoolCode = school.SchoolCode,
            IsActive = school.IsActive
        };

        return Result<SchoolListDto>.Success(result, "Okul getirildi.");
    }

    public async Task<Result> CreateAsync(CreateSchoolDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var nameExists = await _schoolRepository.NameExistsAsync(dto.Name);

        if (nameExists)
            return Result.Failure("Bu okul adı zaten kayıtlı.", 400);

        var school = new School
        {
            Name = dto.Name.Trim(),
            City = dto.City.Trim(),
            District = dto.District.Trim(),
            Address = dto.Address.Trim(),
            Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber) ? null : dto.PhoneNumber.Trim(),
            SchoolCode = await GenerateSchoolCodeAsync(),
            IsActive = true
        };

        await _schoolRepository.CreateAsync(school);

        return Result.Success("Okul başarıyla oluşturuldu.");
    }

    public async Task<Result> UpdateAsync(UpdateSchoolDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var school = await _schoolRepository.GetByIdAsync(dto.Id);

        if (school is null)
            return Result.Failure("Okul bulunamadı.", 404);

        var nameExists = await _schoolRepository.NameExistsForUpdateAsync(dto.Id, dto.Name);

        if (nameExists)
            return Result.Failure("Bu okul adı başka bir okulda kullanılıyor.", 400);

        school.Name = dto.Name.Trim();
        school.City = dto.City.Trim();
        school.District = dto.District.Trim();
        school.Address = dto.Address.Trim();
        school.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
        school.PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber) ? null : dto.PhoneNumber.Trim();
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

    private async Task<string> GenerateSchoolCodeAsync()
    {
        string code;

        do
        {
            code = $"EDU-{Random.Shared.Next(100000, 999999)}";
        }
        while (await _schoolRepository.SchoolCodeExistsAsync(code));

        return code;
    }
}