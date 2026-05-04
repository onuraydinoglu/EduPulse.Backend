using EduPulse.DTOs.Common;
using EduPulse.DTOs.EventMembers;

namespace EduPulse.Business.Abstracts;

public interface IEventMemberService
{
    Task<Result<List<EventMemberListDto>>> GetAllForCurrentUserAsync(string? roleName, string? schoolId);
    Task<Result<List<EventMemberListDto>>> GetByEventIdForCurrentUserAsync(string eventId, string? roleName, string? schoolId);
    Task<Result<List<EventMemberListDto>>> GetByStudentIdForCurrentUserAsync(string studentId, string? roleName, string? schoolId);

    Task<Result> CreateAsync(CreateEventMemberDto dto, string? roleName, string? schoolId);
    Task<Result> UpdatePaymentAsync(UpdateEventMemberPaymentDto dto, string? roleName, string? schoolId);
    Task<Result> DeleteAsync(string id, string? roleName, string? schoolId);
}