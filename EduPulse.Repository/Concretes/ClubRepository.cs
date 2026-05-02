using EduPulse.Entities.Clubs;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Context;
using MongoDB.Driver;

namespace EduPulse.Repository.Concretes;

public class ClubRepository : IClubRepository
{
    private readonly IMongoCollection<Club> _clubs;

    public ClubRepository(MongoDbContext context)
    {
        _clubs = context.Clubs;
    }

    public async Task<List<Club>> GetAllAsync()
    {
        return await _clubs.Find(x => true).ToListAsync();
    }

    public async Task<List<Club>> GetBySchoolIdAsync(string schoolId)
    {
        return await _clubs
            .Find(x => x.SchoolId == schoolId)
            .ToListAsync();
    }

    public async Task<Club?> GetByIdAsync(string id)
    {
        return await _clubs
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Club?> GetBySchoolIdAndNameAsync(string schoolId, string normalizedName)
    {
        return await _clubs
            .Find(x =>
                x.SchoolId == schoolId &&
                x.NormalizedName == normalizedName &&
                x.IsActive)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Club club)
    {
        await _clubs.InsertOneAsync(club);
    }

    public async Task UpdateAsync(Club club)
    {
        club.UpdatedDate = DateTime.UtcNow;

        await _clubs.ReplaceOneAsync(
            x => x.Id == club.Id,
            club
        );
    }

    public async Task DeleteAsync(string id)
    {
        var update = Builders<Club>.Update
            .Set(x => x.IsActive, false)
            .Set(x => x.UpdatedDate, DateTime.UtcNow);

        await _clubs.UpdateOneAsync(
            x => x.Id == id && x.IsActive,
            update
        );
    }
}