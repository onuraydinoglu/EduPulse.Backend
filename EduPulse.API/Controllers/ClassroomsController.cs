using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Classrooms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "SchoolAdmin")]
public class ClassroomsController : ControllerBase
{
    private readonly IClassroomService _classroomService;

    public ClassroomsController(IClassroomService classroomService)
    {
        _classroomService = classroomService;
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

        var result = await _classroomService.GetBySchoolIdAsync(schoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        var result = await _classroomService.GetByIdAsync(id);

        if (result.Data == null || result.Data.SchoolId != schoolId)
            return Forbid("Bu kayda erişim yetkiniz yok.");

        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClassroomDto dto)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        dto.SchoolId = schoolId;

        var result = await _classroomService.CreateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateClassroomDto dto)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        dto.SchoolId = schoolId;

        var result = await _classroomService.UpdateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        var classroom = await _classroomService.GetByIdAsync(id);

        if (classroom.Data == null || classroom.Data.SchoolId != schoolId)
            return Forbid("Bu kaydı silme yetkiniz yok.");

        var result = await _classroomService.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}