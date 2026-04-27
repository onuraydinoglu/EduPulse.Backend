using EduPulse.Entities.Roles;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Context;
using MongoDB.Driver;

namespace EduPulse.Repository.Concretes;

public class RoleRepository : IRoleRepository
{
    private readonly IMongoCollection<Role> _roles;

    public RoleRepository(MongoDbContext context)
    {
        _roles = context.Roles;
    }

    public async Task<List<Role>> GetAllAsync()
    {
        return await _roles.Find(x => true).ToListAsync();
    }

    public async Task<Role?> GetByIdAsync(string id)
    {
        return await _roles.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _roles.Find(x => x.Name == name).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Role role)
    {
        await _roles.InsertOneAsync(role);
    }

    public async Task UpdateAsync(Role role)
    {
        role.UpdatedDate = DateTime.UtcNow;
        await _roles.ReplaceOneAsync(x => x.Id == role.Id, role);
    }

    public async Task DeleteAsync(string id)
    {
        await _roles.DeleteOneAsync(x => x.Id == id);
    }
}