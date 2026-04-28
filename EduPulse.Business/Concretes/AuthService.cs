using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Auth;
using EduPulse.DTOs.Common;
using EduPulse.Repository.Abstracts;

namespace EduPulse.Business.Concretes;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUserRepository userRepository,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);

        if (user is null)
            return Result<LoginResponseDto>.Failure("Email veya şifre hatalı.", 400);

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!isPasswordValid)
            return Result<LoginResponseDto>.Failure("Email veya şifre hatalı.", 400);

        if (!user.IsActive)
            return Result<LoginResponseDto>.Failure("Kullanıcı pasif.", 403);

        var token = _jwtService.CreateToken(user);

        var response = new LoginResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            RoleId = user.RoleId,
            RoleName = user.RoleName,
            SchoolId = user.SchoolId,
            Token = token
        };

        return Result<LoginResponseDto>.Success(response);
    }
}