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

    [HttpGet("teachers")]
    [Authorize(Roles = "superadmin,schooladmin")]
    public async Task<IActionResult> GetTeachers()
    {
        var schoolId = RoleName == "superadmin" ? null : SchoolId;

        var result = await _userService.GetTeachersAsync(schoolId);
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

    [HttpGet("students")]
    [Authorize(Roles = "superadmin,schooladmin")]
    public async Task<IActionResult> GetStudents()
    {
        var schoolId = RoleName == "superadmin" ? null : SchoolId;

        var result = await _userService.GetStudentsAsync(schoolId);
        return StatusCode(result.StatusCode, result);
    }


    [HttpPost("teacher")]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> CreateTeacher(CreateUserDto dto)
    {
        var result = await _userService.CreateUserAsync(dto, SchoolId, "teacher");
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("officer")]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> CreateOfficer(CreateUserDto dto)
    {
        var result = await _userService.CreateUserAsync(dto, SchoolId, "officer");
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("student")]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> CreateStudent(CreateUserDto dto)
    {
        var result = await _userService.CreateUserAsync(dto, SchoolId, "student");
        return StatusCode(result.StatusCode, result);
    }


    [HttpPut]
    [Authorize(Roles = "superadmin,schooladmin")]
    public async Task<IActionResult> Update(UpdateUserDto dto)
    {
        var result = await _userService.UpdateForCurrentUserAsync(dto, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "superadmin,schooladmin")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _userService.DeleteForCurrentUserAsync(id, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }
}