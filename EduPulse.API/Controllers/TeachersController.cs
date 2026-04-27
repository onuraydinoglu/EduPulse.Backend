using System.Security.Claims;
using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Teachers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    private string? GetSchoolId()
    {
        return User.FindFirst("schoolId")?.Value;
    }

    [HttpGet]
    [Authorize(Roles = "SchoolAdmin")]
    public async Task<IActionResult> GetAll()
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        var result = await _teacherService.GetBySchoolIdAsync(schoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "SchoolAdmin")]
    public async Task<IActionResult> GetById(string id)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        var result = await _teacherService.GetByIdAsync(id);

        if (result.Data == null || result.Data.SchoolId != schoolId)
            return Forbid("Bu kayda erişim yetkiniz yok.");

        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Roles = "SchoolAdmin")]
    public async Task<IActionResult> Create(CreateTeacherDto dto)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        dto.SchoolId = schoolId;

        var result = await _teacherService.CreateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    [Authorize(Roles = "SchoolAdmin")]
    public async Task<IActionResult> Update(UpdateTeacherDto dto)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        dto.SchoolId = schoolId;

        var result = await _teacherService.UpdateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SchoolAdmin")]
    public async Task<IActionResult> Delete(string id)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        var teacher = await _teacherService.GetByIdAsync(id);

        if (teacher.Data == null || teacher.Data.SchoolId != schoolId)
            return Forbid("Bu kaydı silme yetkiniz yok.");

        var result = await _teacherService.DeleteAsync(id);

        return StatusCode(result.StatusCode, result);
    }
}