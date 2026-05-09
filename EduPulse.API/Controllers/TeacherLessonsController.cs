using EduPulse.Business.Abstracts;
using EduPulse.DTOs.TeacherLessons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TeacherLessonsController : ControllerBase
{
    private readonly ITeacherLessonService _teacherLessonService;

    public TeacherLessonsController(ITeacherLessonService teacherLessonService)
    {
        _teacherLessonService = teacherLessonService;
    }

    private string? RoleName => User.FindFirst(ClaimTypes.Role)?.Value;
    private string? SchoolId => User.FindFirst("schoolId")?.Value;
    private string? CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    [HttpGet]
    [Authorize(Roles = "superadmin,schooladmin,teacher")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _teacherLessonService.GetAllForCurrentUserAsync(
            RoleName,
            SchoolId,
            CurrentUserId
        );

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "superadmin,schooladmin,teacher")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _teacherLessonService.GetByIdForCurrentUserAsync(
            id,
            RoleName,
            SchoolId,
            CurrentUserId
        );

        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> Create(CreateTeacherLessonDto dto)
    {
        var result = await _teacherLessonService.CreateAsync(dto, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> Update(UpdateTeacherLessonDto dto)
    {
        var result = await _teacherLessonService.UpdateAsync(dto, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _teacherLessonService.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}