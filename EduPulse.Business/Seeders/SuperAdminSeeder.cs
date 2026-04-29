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
        // 🔹 1. TÜM ROLLERİ EKLE
        var roleNames = new List<string>
        {
            "superadmin",
            "schooladmin",
            "teacher",
            "officer",
            "student"
        };

        var roles = new Dictionary<string, Role>();

        foreach (var roleName in roleNames)
        {
            var existingRole = await _roleRepository.GetByNameAsync(roleName);

            if (existingRole is null)
            {
                var newRole = new Role
                {
                    Name = roleName,
                    IsActive = true
                };

                await _roleRepository.CreateAsync(newRole);
                roles[roleName] = newRole;
            }
            else
            {
                roles[roleName] = existingRole;
            }
        }

        // 🔹 2. SUPERADMIN ROLÜNÜ AL
        var superAdminRole = roles["superadmin"];

        // 🔹 3. SUPERADMIN USERLARI
        var admins = new List<User>
        {
            new User
            {
                FirstName = "Onur",
                LastName = "Aydınoğlu",
                Email = "onur@gmail.com",
                PhoneNumber = "05456565712",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                RoleId = superAdminRole.Id,
                RoleName = superAdminRole.Name,
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
                RoleId = superAdminRole.Id,
                RoleName = superAdminRole.Name,
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