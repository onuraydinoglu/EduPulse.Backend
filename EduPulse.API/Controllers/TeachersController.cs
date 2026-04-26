using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Teachers;
using Microsoft.AspNetCore.Mvc;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeachersController : ControllerBase
{
    private readonly ITeacherService _teacherService;

    public TeachersController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var teachers = await _teacherService.GetAllAsync();
        return Ok(teachers);
    }

    [HttpGet("school/{schoolId}")]
    public async Task<IActionResult> GetBySchoolId(string schoolId)
    {
        var teachers = await _teacherService.GetBySchoolIdAsync(schoolId);
        return Ok(teachers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var teacher = await _teacherService.GetByIdAsync(id);

        if (teacher is null)
            return NotFound("Öğretmen bulunamadı.");

        return Ok(teacher);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTeacherDto dto)
    {
        await _teacherService.CreateAsync(dto);
        return Ok("Öğretmen başarıyla oluşturuldu.");
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateTeacherDto dto)
    {
        await _teacherService.UpdateAsync(dto);
        return Ok("Öğretmen başarıyla güncellendi.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _teacherService.DeleteAsync(id);
        return Ok("Öğretmen başarıyla silindi.");
    }
}