using EduPulse.DTOs.Common;
using EduPulse.DTOs.PersonalNotes;

namespace EduPulse.Business.Abstracts;

public interface IPersonalNoteService
{
    Task<Result<List<PersonalNoteListDto>>> GetAllForCurrentUserAsync(string? schoolId, string? userId);
    Task<Result<PersonalNoteListDto>> GetByIdForCurrentUserAsync(string id, string? schoolId, string? userId);

    Task<Result> CreateAsync(CreatePersonalNoteDto dto, string? schoolId, string? userId);
    Task<Result> UpdateAsync(UpdatePersonalNoteDto dto, string? schoolId, string? userId);
    Task<Result> DeleteAsync(string id, string? schoolId, string? userId);
}