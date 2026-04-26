using EduPulse.Entities.Parents;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Context;
using MongoDB.Driver;

namespace EduPulse.Repository.Concretes;

public class ParentRepository : IParentRepository
{
    private readonly IMongoCollection<Parent> _parents;

    public ParentRepository(MongoDbContext context)
    {
        _parents = context.Parents;
    }

    public async Task<List<Parent>> GetAllAsync()
    {
        return await _parents.Find(x => x.IsActive).ToListAsync();
    }

    public async Task<List<Parent>> GetBySchoolIdAsync(string schoolId)
    {
        return await _parents
            .Find(x => x.SchoolId == schoolId && x.IsActive)
            .ToListAsync();
    }

    public async Task<Parent?> GetByIdAsync(string id)
    {
        return await _parents.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Parent parent)
    {
        await _parents.InsertOneAsync(parent);
    }

    public async Task UpdateAsync(Parent parent)
    {
        parent.UpdatedDate = DateTime.UtcNow;

        await _parents.ReplaceOneAsync(
            x => x.Id == parent.Id,
            parent
        );
    }

    public async Task DeleteAsync(string id)
    {
        var update = Builders<Parent>.Update
            .Set(x => x.IsActive, false)
            .Set(x => x.UpdatedDate, DateTime.UtcNow);

        await _parents.UpdateOneAsync(x => x.Id == id, update);
    }
}