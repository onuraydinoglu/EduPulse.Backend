using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Lessons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonsController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    private string? RoleName => User.FindFirst(ClaimTypes.Role)?.Value;
    private string? SchoolId => User.FindFirst("schoolId")?.Value;

    [HttpGet]
    [Authorize(Roles = "superadmin,schooladmin,officer,teacher")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _lessonService.GetAllForCurrentUserAsync(RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "superadmin,schooladmin,officer,teacher")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _lessonService.GetByIdForCurrentUserAsync(id, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Roles = "schooladmin,officer")]
    public async Task<IActionResult> Create(CreateLessonDto dto)
    {
        var result = await _lessonService.CreateAsync(dto, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    [Authorize(Roles = "schooladmin,officer")]
    public async Task<IActionResult> Update(UpdateLessonDto dto)
    {
        var result = await _lessonService.UpdateAsync(dto, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "schooladmin,officer")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _lessonService.DeleteAsync(id, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }
}