using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Students;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    private string? GetCurrentSchoolId()
    {
        return User.FindFirst("schoolId")?.Value;
    }

    private string? GetCurrentRoleName()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value;
    }

    [HttpGet]
    [Authorize(Roles = "schooladmin,teacher")]
    public async Task<IActionResult> GetAll()
    {
        var currentRoleName = GetCurrentRoleName();
        var currentSchoolId = GetCurrentSchoolId();

        var result = await _studentService.GetAllForCurrentUserAsync(
            currentRoleName,
            currentSchoolId
        );

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "schooladmin,teacher")]
    public async Task<IActionResult> GetById(string id)
    {
        var currentRoleName = GetCurrentRoleName();
        var currentSchoolId = GetCurrentSchoolId();

        var result = await _studentService.GetByIdForCurrentUserAsync(
            id,
            currentRoleName,
            currentSchoolId
        );

        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> Create(CreateStudentDto dto)
    {
        var currentRoleName = GetCurrentRoleName();
        var currentSchoolId = GetCurrentSchoolId();

        var result = await _studentService.CreateForCurrentUserAsync(
            dto,
            currentRoleName,
            currentSchoolId
        );

        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> Update(UpdateStudentDto dto)
    {
        var currentRoleName = GetCurrentRoleName();
        var currentSchoolId = GetCurrentSchoolId();

        var result = await _studentService.UpdateForCurrentUserAsync(
            dto,
            currentRoleName,
            currentSchoolId
        );

        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> Delete(string id)
    {
        var currentRoleName = GetCurrentRoleName();
        var currentSchoolId = GetCurrentSchoolId();

        var result = await _studentService.DeleteForCurrentUserAsync(
            id,
            currentRoleName,
            currentSchoolId
        );

        return StatusCode(result.StatusCode, result);
    }
}