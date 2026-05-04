using EduPulse.Entities.EventMembers;

namespace EduPulse.Repository.Abstracts;

public interface IEventMemberRepository
{
    Task<List<EventMember>> GetAllAsync();
    Task<List<EventMember>> GetBySchoolIdAsync(string schoolId);
    Task<List<EventMember>> GetByEventIdAsync(string eventId);
    Task<List<EventMember>> GetByStudentIdAsync(string studentId);

    Task<EventMember?> GetByIdAsync(string id);
    Task<EventMember?> GetByEventIdAndStudentIdAsync(string eventId, string studentId);

    Task<int> GetActiveMemberCountByEventIdAsync(string eventId);

    Task CreateAsync(EventMember eventMember);
    Task UpdatePaymentAsync(EventMember eventMember);
    Task DeleteAsync(string id);
}