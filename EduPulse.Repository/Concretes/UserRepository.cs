using EduPulse.Entities.Users;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Context;
using MongoDB.Driver;

namespace EduPulse.Repository.Concretes;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(MongoDbContext context)
    {
        _users = context.Users;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _users.Find(x => true).ToListAsync();
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _users.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLower();

        return await _users
            .Find(x => x.Email.ToLower() == normalizedEmail)
            .FirstOrDefaultAsync();
    }

    public async Task<List<User>> GetBySchoolIdAsync(string schoolId)
    {
        return await _users
            .Find(x => x.SchoolId == schoolId)
            .ToListAsync();
    }

    public async Task<List<User>> GetByRoleNameAsync(string roleName)
    {
        var normalizedRoleName = roleName.Trim().ToLower();

        return await _users
            .Find(x => x.RoleName.ToLower() == normalizedRoleName)
            .ToListAsync();
    }

    public async Task<List<User>> GetBySchoolIdAndRoleNameAsync(string schoolId, string roleName)
    {
        var normalizedRoleName = roleName.Trim().ToLower();

        return await _users
            .Find(x =>
                x.SchoolId == schoolId &&
                x.RoleName.ToLower() == normalizedRoleName)
            .ToListAsync();
    }

    public async Task<bool> ExistsSchoolAdminAsync(string schoolId)
    {
        return await _users
            .Find(x =>
                x.SchoolId == schoolId &&
                x.RoleName.ToLower() == "schooladmin")
            .AnyAsync();
    }

    public async Task CreateAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        await _users.ReplaceOneAsync(x => x.Id == user.Id, user);
    }

    public async Task DeleteAsync(string id)
    {
        await _users.DeleteOneAsync(x => x.Id == id);
    }


}