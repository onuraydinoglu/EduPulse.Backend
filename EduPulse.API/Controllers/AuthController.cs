using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register-school")]
    public async Task<IActionResult> RegisterSchool(RegisterSchoolDto dto)
    {
        var result = await _authService.RegisterSchoolAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return StatusCode(result.StatusCode, result);
    }
}