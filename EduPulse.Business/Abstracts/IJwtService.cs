using EduPulse.Entities.Users;

namespace EduPulse.Business.Abstracts;

public interface IJwtService
{
    string CreateToken(User user);
}