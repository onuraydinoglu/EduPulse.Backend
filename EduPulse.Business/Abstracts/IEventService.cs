using EduPulse.DTOs.Common;
using EduPulse.DTOs.Events;

namespace EduPulse.Business.Abstracts;

public interface IEventService
{
    Task<Result<List<EventListDto>>> GetAllForCurrentUserAsync(string? roleName, string? schoolId);
    Task<Result<EventListDto>> GetByIdForCurrentUserAsync(string id, string? roleName, string? schoolId);
    Task<Result> CreateAsync(CreateEventDto dto, string? roleName, string? schoolId);
    Task<Result> UpdateAsync(UpdateEventDto dto, string? roleName, string? schoolId);
    Task<Result> DeleteAsync(string id, string? roleName, string? schoolId);
}