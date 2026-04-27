using EduPulse.Entities.Schools;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Context;
using MongoDB.Driver;

namespace EduPulse.Repository.Concretes;

public class SchoolRepository : ISchoolRepository
{
    private readonly IMongoCollection<School> _schools;

    public SchoolRepository(MongoDbContext context)
    {
        _schools = context.Schools;
    }

    public async Task<List<School>> GetAllAsync()
    {
        return await _schools.Find(x => true).ToListAsync();
    }

    public async Task<School?> GetByIdAsync(string id)
    {
        return await _schools.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<School?> GetByEmailAsync(string email)
    {
        return await _schools.Find(x => x.Email == email).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(School school)
    {
        await _schools.InsertOneAsync(school);
    }

    public async Task UpdateAsync(School school)
    {
        school.UpdatedDate = DateTime.UtcNow;

        await _schools.ReplaceOneAsync(
            x => x.Id == school.Id,
            school
        );
    }

    public async Task DeleteAsync(string id)
    {
        await _schools.DeleteOneAsync(x => x.Id == id);
    }
}