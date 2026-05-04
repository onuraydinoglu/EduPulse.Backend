using EduPulse.Entities.EventMembers;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Context;
using MongoDB.Driver;

namespace EduPulse.Repository.Concretes;

public class EventMemberRepository : IEventMemberRepository
{
    private readonly IMongoCollection<EventMember> _eventMembers;

    public EventMemberRepository(MongoDbContext context)
    {
        _eventMembers = context.EventMembers;
    }

    public async Task<List<EventMember>> GetAllAsync()
    {
        return await _eventMembers
            .Find(x => x.IsActive)
            .SortByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<EventMember>> GetBySchoolIdAsync(string schoolId)
    {
        return await _eventMembers
            .Find(x => x.SchoolId == schoolId && x.IsActive)
            .SortByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<EventMember>> GetByEventIdAsync(string eventId)
    {
        return await _eventMembers
            .Find(x => x.EventId == eventId && x.IsActive)
            .SortByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<EventMember>> GetByStudentIdAsync(string studentId)
    {
        return await _eventMembers
            .Find(x => x.StudentId == studentId && x.IsActive)
            .SortByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<EventMember?> GetByIdAsync(string id)
    {
        return await _eventMembers
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<EventMember?> GetByEventIdAndStudentIdAsync(string eventId, string studentId)
    {
        return await _eventMembers
            .Find(x =>
                x.EventId == eventId &&
                x.StudentId == studentId &&
                x.IsActive)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetActiveMemberCountByEventIdAsync(string eventId)
    {
        return (int)await _eventMembers
            .CountDocumentsAsync(x => x.EventId == eventId && x.IsActive);
    }

    public async Task CreateAsync(EventMember eventMember)
    {
        await _eventMembers.InsertOneAsync(eventMember);
    }

    public async Task UpdatePaymentAsync(EventMember eventMember)
    {
        var update = Builders<EventMember>.Update
            .Set(x => x.IsPaid, eventMember.IsPaid)
            .Set(x => x.PaidAmount, eventMember.PaidAmount)
            .Set(x => x.UpdatedDate, DateTime.UtcNow);

        await _eventMembers.UpdateOneAsync(
            x => x.Id == eventMember.Id,
            update);
    }

    public async Task DeleteAsync(string id)
    {
        var update = Builders<EventMember>.Update
            .Set(x => x.IsActive, false)
            .Set(x => x.UpdatedDate, DateTime.UtcNow);

        await _eventMembers.UpdateOneAsync(
            x => x.Id == id && x.IsActive,
            update);
    }
}