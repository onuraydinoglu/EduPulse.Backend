using EduPulse.DTOs.Auth;
using EduPulse.DTOs.Common;

namespace EduPulse.Business.Abstracts;

public interface IAuthService
{
    Task<Result> RegisterUserAsync(RegisterUserDto dto);
    Task<Result<LoginResponseDto>> LoginAsync(LoginDto dto);
}