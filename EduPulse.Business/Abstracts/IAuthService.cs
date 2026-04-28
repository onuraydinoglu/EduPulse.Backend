using EduPulse.DTOs.Auth;
using EduPulse.DTOs.Common;

namespace EduPulse.Business.Abstracts;

public interface IAuthService
{
    Task<Result<LoginResponseDto>> LoginAsync(LoginDto dto);
}