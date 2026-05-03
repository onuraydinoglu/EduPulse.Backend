using EduPulse.Entities.Events;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Context;
using MongoDB.Driver;

namespace EduPulse.Repository.Concretes;

public class EventRepository : IEventRepository
{
    private readonly IMongoCollection<Event> _events;

    public EventRepository(MongoDbContext context)
    {
        _events = context.Events;
    }

    public async Task<List<Event>> GetAllAsync()
    {
        return await _events.Find(x => true).ToListAsync();
    }

    public async Task<List<Event>> GetBySchoolIdAsync(string schoolId)
    {
        return await _events
            .Find(x => x.SchoolId == schoolId)
            .ToListAsync();
    }

    public async Task<Event?> GetByIdAsync(string id)
    {
        return await _events
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Event?> GetBySchoolIdAndNameAsync(string schoolId, string normalizedName)
    {
        return await _events
            .Find(x =>
                x.SchoolId == schoolId &&
                x.NormalizedName == normalizedName &&
                x.IsActive)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Event eventEntity)
    {
        await _events.InsertOneAsync(eventEntity);
    }

    public async Task UpdateAsync(Event eventEntity)
    {
        eventEntity.UpdatedDate = DateTime.UtcNow;

        await _events.ReplaceOneAsync(
            x => x.Id == eventEntity.Id,
            eventEntity
        );
    }

    public async Task DeleteAsync(string id)
    {
        var update = Builders<Event>.Update
            .Set(x => x.IsActive, false)
            .Set(x => x.UpdatedDate, DateTime.UtcNow);

        await _events.UpdateOneAsync(
            x => x.Id == id && x.IsActive,
            update
        );
    }
}