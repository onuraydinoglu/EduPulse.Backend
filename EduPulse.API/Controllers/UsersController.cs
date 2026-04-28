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

    [HttpGet]
    [Authorize(Roles = "superadmin,schooladmin")]
    public async Task<IActionResult> GetAll()
    {
        var roleName = User.FindFirst(ClaimTypes.Role)?.Value;
        var schoolId = User.FindFirst("schoolId")?.Value;

        if (roleName == "superadmin")
        {
            var allResult = await _userService.GetAllAsync();
            return StatusCode(allResult.StatusCode, allResult);
        }

        var schoolResult = await _userService.GetBySchoolIdAsync(schoolId!);
        return StatusCode(schoolResult.StatusCode, schoolResult);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _userService.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("school/{schoolId}")]
    public async Task<IActionResult> GetBySchoolId(string schoolId)
    {
        var result = await _userService.GetBySchoolIdAsync(schoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("teacher")]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> CreateTeacher(CreateUserDto dto)
    {
        var schoolId = User.FindFirst("schoolId")?.Value;

        var result = await _userService.CreateUserAsync(dto, schoolId, "teacher");

        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("officer")]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> CreateOfficer(CreateUserDto dto)
    {
        var schoolId = User.FindFirst("schoolId")?.Value;

        var result = await _userService.CreateUserAsync(dto, schoolId, "officer");

        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateUserDto dto)
    {
        var result = await _userService.UpdateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _userService.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}