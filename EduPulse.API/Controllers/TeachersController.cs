using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Teachers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TeachersController : ControllerBase
{
    private readonly ITeacherService _teacherService;

    public TeachersController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    private string? RoleName => User.FindFirst(ClaimTypes.Role)?.Value;
    private string? SchoolId => User.FindFirst("schoolId")?.Value;

    [HttpGet]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _teacherService.GetAllForCurrentUserAsync(RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _teacherService.GetByIdForCurrentUserAsync(id, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> Create(CreateTeacherDto dto)
    {
        var result = await _teacherService.CreateForCurrentUserAsync(dto, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> Update(UpdateTeacherDto dto)
    {
        var result = await _teacherService.UpdateForCurrentUserAsync(dto, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _teacherService.DeleteForCurrentUserAsync(id, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }
}