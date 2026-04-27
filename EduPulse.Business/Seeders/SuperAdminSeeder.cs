using EduPulse.Entities.Roles;
using EduPulse.Entities.Users;
using EduPulse.Repository.Abstracts;

namespace EduPulse.Business.Seeders;

public class SuperAdminSeeder
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public SuperAdminSeeder(
        IUserRepository userRepository,
        IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task SeedAsync()
    {
        var role = await _roleRepository.GetByNameAsync("SuperAdmin");

        if (role is null)
        {
            role = new Role
            {
                Name = "SuperAdmin",
                IsActive = true
            };

            await _roleRepository.CreateAsync(role);
        }

        var admin = await _userRepository.GetByEmailAsync("admin@edupulse.com");

        if (admin is null)
        {
            var user = new User
            {
                FullName = "Super Admin",
                Email = "admin@edupulse.com",
                PasswordHash = HashPassword("123456"),
                RoleId = role.Id,
                RoleName = role.Name,
                IsActive = true
            };

            await _userRepository.CreateAsync(user);
        }
    }

    private string HashPassword(string password)
    {
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }
}