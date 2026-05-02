using EduPulse.Entities.ClubMembers;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Context;
using MongoDB.Driver;

namespace EduPulse.Repository.Concretes;

public class ClubMemberRepository : IClubMemberRepository
{
    private readonly IMongoCollection<ClubMember> _clubMembers;

    public ClubMemberRepository(MongoDbContext context)
    {
        _clubMembers = context.ClubMembers;
    }

    public async Task<List<ClubMember>> GetByClubIdAsync(string clubId)
    {
        return await _clubMembers
            .Find(x => x.ClubId == clubId && x.IsActive)
            .ToListAsync();
    }

    public async Task<List<ClubMember>> GetByStudentIdAsync(string studentId)
    {
        return await _clubMembers
            .Find(x => x.StudentId == studentId && x.IsActive)
            .ToListAsync();
    }

    public async Task<ClubMember?> GetActiveByClubIdAndStudentIdAsync(string clubId, string studentId)
    {
        return await _clubMembers
            .Find(x =>
                x.ClubId == clubId &&
                x.StudentId == studentId &&
                x.IsActive)
            .FirstOrDefaultAsync();
    }

    public async Task<ClubMember?> GetAnyByClubIdAndStudentIdAsync(string clubId, string studentId)
    {
        return await _clubMembers
            .Find(x =>
                x.ClubId == clubId &&
                x.StudentId == studentId)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetActiveMemberCountByClubIdAsync(string clubId)
    {
        return (int)await _clubMembers
            .CountDocumentsAsync(x => x.ClubId == clubId && x.IsActive);
    }

    public async Task CreateAsync(ClubMember clubMember)
    {
        await _clubMembers.InsertOneAsync(clubMember);
    }

    public async Task ReactivateAsync(string clubId, string studentId)
    {
        var update = Builders<ClubMember>.Update
            .Set(x => x.IsActive, true)
            .Set(x => x.UpdatedDate, DateTime.UtcNow);

        await _clubMembers.UpdateOneAsync(
            x => x.ClubId == clubId && x.StudentId == studentId,
            update
        );
    }

    public async Task DeleteAsync(string clubId, string studentId)
    {
        var update = Builders<ClubMember>.Update
            .Set(x => x.IsActive, false)
            .Set(x => x.UpdatedDate, DateTime.UtcNow);

        await _clubMembers.UpdateOneAsync(
            x =>
                x.ClubId == clubId &&
                x.StudentId == studentId &&
                x.IsActive,
            update
        );
    }
}