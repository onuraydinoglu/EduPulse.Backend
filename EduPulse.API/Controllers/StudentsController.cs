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

    private string? RoleName => User.FindFirst(ClaimTypes.Role)?.Value;
    private string? SchoolId => User.FindFirst("schoolId")?.Value;

    [HttpGet]
    [Authorize(Roles = "schooladmin,teacher")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _studentService.GetAllForCurrentUserAsync(RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "schooladmin,teacher")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _studentService.GetByIdForCurrentUserAsync(id, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> Create(CreateStudentDto dto)
    {
        var result = await _studentService.CreateForCurrentUserAsync(dto, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> Update(UpdateStudentDto dto)
    {
        var result = await _studentService.UpdateForCurrentUserAsync(dto, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _studentService.DeleteForCurrentUserAsync(id, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }
}