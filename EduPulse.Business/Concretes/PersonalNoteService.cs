using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Common;
using EduPulse.DTOs.PersonalNotes;
using EduPulse.Entities.PersonalNotes;
using EduPulse.Repository.Abstracts;
using FluentValidation;

namespace EduPulse.Business.Concretes;

public class PersonalNoteService : IPersonalNoteService
{
    private readonly IPersonalNoteRepository _personalNoteRepository;
    private readonly IValidator<CreatePersonalNoteDto> _createValidator;
    private readonly IValidator<UpdatePersonalNoteDto> _updateValidator;

    public PersonalNoteService(
        IPersonalNoteRepository personalNoteRepository,
        IValidator<CreatePersonalNoteDto> createValidator,
        IValidator<UpdatePersonalNoteDto> updateValidator)
    {
        _personalNoteRepository = personalNoteRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<List<PersonalNoteListDto>>> GetAllForCurrentUserAsync(
        string? schoolId,
        string? userId)
    {
        if (string.IsNullOrWhiteSpace(schoolId))
            return Result<List<PersonalNoteListDto>>.Failure("Okul bilgisi bulunamadı.", 400);

        if (string.IsNullOrWhiteSpace(userId))
            return Result<List<PersonalNoteListDto>>.Failure("Kullanıcı bilgisi bulunamadı.", 400);

        var notes = await _personalNoteRepository.GetByUserAsync(schoolId, userId);

        var dtoList = notes.Select(x => new PersonalNoteListDto
        {
            Id = x.Id,
            Title = x.Title,
            Content = x.Content,
            IsActive = x.IsActive,
            CreatedDate = x.CreatedDate
        }).ToList();

        return Result<List<PersonalNoteListDto>>.Success(dtoList);
    }

    public async Task<Result<PersonalNoteListDto>> GetByIdForCurrentUserAsync(
        string id,
        string? schoolId,
        string? userId)
    {
        if (string.IsNullOrWhiteSpace(id))
            return Result<PersonalNoteListDto>.Failure("Not Id boş olamaz.", 400);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result<PersonalNoteListDto>.Failure("Okul bilgisi bulunamadı.", 400);

        if (string.IsNullOrWhiteSpace(userId))
            return Result<PersonalNoteListDto>.Failure("Kullanıcı bilgisi bulunamadı.", 400);

        var note = await _personalNoteRepository.GetOwnedNoteAsync(id, schoolId, userId);

        if (note == null)
            return Result<PersonalNoteListDto>.Failure("Not bulunamadı veya bu işlem için yetkiniz yok.", 404);

        var dto = new PersonalNoteListDto
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            IsActive = note.IsActive,
            CreatedDate = note.CreatedDate
        };

        return Result<PersonalNoteListDto>.Success(dto);
    }

    public async Task<Result> CreateAsync(
        CreatePersonalNoteDto dto,
        string? schoolId,
        string? userId)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            var errorMessage = validationResult.Errors.First().ErrorMessage;
            return Result.Failure(errorMessage, 400);
        }

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        if (string.IsNullOrWhiteSpace(userId))
            return Result.Failure("Kullanıcı bilgisi bulunamadı.", 400);

        var note = new PersonalNote
        {
            SchoolId = schoolId,
            UserId = userId,
            Title = dto.Title.Trim(),
            Content = dto.Content.Trim(),
            IsActive = true
        };

        await _personalNoteRepository.CreateAsync(note);

        return Result.Success("Not başarıyla oluşturuldu.", 201);
    }

    public async Task<Result> UpdateAsync(
        UpdatePersonalNoteDto dto,
        string? schoolId,
        string? userId)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            var errorMessage = validationResult.Errors.First().ErrorMessage;
            return Result.Failure(errorMessage, 400);
        }

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        if (string.IsNullOrWhiteSpace(userId))
            return Result.Failure("Kullanıcı bilgisi bulunamadı.", 400);

        var note = await _personalNoteRepository.GetOwnedNoteAsync(dto.Id, schoolId, userId);

        if (note == null)
            return Result.Failure("Not bulunamadı veya bu işlem için yetkiniz yok.", 404);

        note.Title = dto.Title.Trim();
        note.Content = dto.Content.Trim();
        note.IsActive = dto.IsActive;
        note.UpdatedDate = DateTime.UtcNow;

        await _personalNoteRepository.UpdateAsync(note);

        return Result.Success("Not başarıyla güncellendi.", 200);
    }

    public async Task<Result> DeleteAsync(
        string id,
        string? schoolId,
        string? userId)
    {
        if (string.IsNullOrWhiteSpace(id))
            return Result.Failure("Not Id boş olamaz.", 400);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        if (string.IsNullOrWhiteSpace(userId))
            return Result.Failure("Kullanıcı bilgisi bulunamadı.", 400);

        var note = await _personalNoteRepository.GetOwnedNoteAsync(id, schoolId, userId);

        if (note == null)
            return Result.Failure("Not bulunamadı veya bu işlem için yetkiniz yok.", 404);

        await _personalNoteRepository.DeleteAsync(id);

        return Result.Success("Not başarıyla silindi.", 200);
    }
}