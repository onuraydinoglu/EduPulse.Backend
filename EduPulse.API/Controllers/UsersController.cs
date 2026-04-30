using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    private string? RoleName => User.FindFirst(ClaimTypes.Role)?.Value;
    private string? SchoolId => User.FindFirst("schoolId")?.Value;

    [HttpGet]
    [Authorize(Roles = "superadmin,schooladmin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _userService.GetAllForCurrentUserAsync(RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "superadmin,schooladmin")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _userService.GetByIdForCurrentUserAsync(id, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("officers")]
    [Authorize(Roles = "superadmin,schooladmin")]
    public async Task<IActionResult> GetOfficers()
    {
        var schoolId = RoleName == "superadmin" ? null : SchoolId;

        var result = await _userService.GetOfficersAsync(schoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("officer")]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> CreateOfficer(CreateUserDto dto)
    {
        var result = await _userService.CreateUserAsync(dto, SchoolId, "officer");
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("officer")]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> UpdateOfficer(UpdateUserDto dto)
    {
        var result = await _userService.UpdateForCurrentUserAsync(
            dto,
            RoleName,
            SchoolId,
            "officer"
        );

        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("officer/{id}")]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> DeleteOfficer(string id)
    {
        var result = await _userService.DeleteForCurrentUserAsync(
            id,
            RoleName,
            SchoolId,
            "officer"
        );

        return StatusCode(result.StatusCode, result);
    }
}