using EduPulse.Entities.Schools;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Context;
using MongoDB.Driver;

namespace EduPulse.Repository.Concretes;

public class SchoolRepository : ISchoolRepository
{
    private readonly IMongoCollection<School> _collection;

    public SchoolRepository(MongoDbContext context)
    {
        _collection = context.Schools;
    }

    public async Task<List<School>> GetAllAsync()
    {
        return await _collection
            .Find(x => true)
            .ToListAsync();
    }

    public async Task<School?> GetByIdAsync(string id)
    {
        return await _collection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<School?> GetBySchoolCodeAsync(string schoolCode)
    {
        return await _collection
            .Find(x => x.SchoolCode == schoolCode)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> SchoolCodeExistsAsync(string schoolCode)
    {
        return await _collection
            .Find(x => x.SchoolCode == schoolCode)
            .AnyAsync();
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _collection
            .Find(x => x.Name.ToLower() == name.ToLower())
            .AnyAsync();
    }

    public async Task<bool> NameExistsForUpdateAsync(string id, string name)
    {
        return await _collection
            .Find(x => x.Id != id && x.Name.ToLower() == name.ToLower())
            .AnyAsync();
    }

    public async Task CreateAsync(School school)
    {
        await _collection.InsertOneAsync(school);
    }

    public async Task UpdateAsync(School school)
    {
        await _collection.ReplaceOneAsync(x => x.Id == school.Id, school);
    }

    public async Task DeleteAsync(string id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }
}