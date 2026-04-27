using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Parents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "schooladmin")]
public class ParentsController : ControllerBase
{
    private readonly IParentService _parentService;

    public ParentsController(IParentService parentService)
    {
        _parentService = parentService;
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

        var result = await _parentService.GetBySchoolIdAsync(schoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        var result = await _parentService.GetByIdAsync(id);

        if (result.Data == null || result.Data.SchoolId != schoolId)
            return Forbid("Bu kayda erişim yetkiniz yok.");

        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateParentDto dto)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        dto.SchoolId = schoolId;

        var result = await _parentService.CreateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateParentDto dto)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        dto.SchoolId = schoolId;

        var result = await _parentService.UpdateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        var parent = await _parentService.GetByIdAsync(id);

        if (parent.Data == null || parent.Data.SchoolId != schoolId)
            return Forbid("Bu kaydı silme yetkiniz yok.");

        var result = await _parentService.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}