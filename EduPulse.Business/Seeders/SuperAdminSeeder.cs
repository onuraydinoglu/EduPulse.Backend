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
        var role = await _roleRepository.GetByNameAsync("superadmin");

        if (role is null)
        {
            role = new Role
            {
                Name = "superadmin",
                IsActive = true
            };

            await _roleRepository.CreateAsync(role);
        }

        var admins = new List<User>
        {
            new User
            {
                FirstName = "Onur",
                LastName = "Aydınoğlu",
                Email = "onur@gmail.com",
                PhoneNumber = "05456565712",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                RoleId = role.Id,
                RoleName = role.Name,
                SchoolId = null,
                IsActive = true
            },
            new User
            {
                FirstName = "Nisa",
                LastName = "Işık",
                Email = "nisa@gmail.com",
                PhoneNumber = "05308414903",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                RoleId = role.Id,
                RoleName = role.Name,
                SchoolId = null,
                IsActive = true
            }
        };

        foreach (var user in admins)
        {
            var existingUser = await _userRepository.GetByEmailAsync(user.Email);

            if (existingUser is null)
            {
                await _userRepository.CreateAsync(user);
            }
        }
    }
}