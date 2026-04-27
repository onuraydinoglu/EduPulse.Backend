using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Lessons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "SchoolAdmin")]
public class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonsController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    private string? GetSchoolId()
    {
        return User.FindFirst("schoolId")?.Value;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        var result = await _lessonService.GetBySchoolIdAsync(schoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        var result = await _lessonService.GetByIdAsync(id);

        if (result.Data == null || result.Data.SchoolId != schoolId)
            return Forbid("Bu kayda erişim yetkiniz yok.");

        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateLessonDto dto)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        dto.SchoolId = schoolId;

        var result = await _lessonService.CreateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateLessonDto dto)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        dto.SchoolId = schoolId;

        var result = await _lessonService.UpdateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        var lesson = await _lessonService.GetByIdAsync(id);

        if (lesson.Data == null || lesson.Data.SchoolId != schoolId)
            return Forbid("Bu kaydı silme yetkiniz yok.");

        var result = await _lessonService.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}