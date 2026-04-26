using EduPulse.Business.Abstracts;
using EduPulse.DTOs.TeacherLessons;
using Microsoft.AspNetCore.Mvc;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeacherLessonsController : ControllerBase
{
    private readonly ITeacherLessonService _service;

    public TeacherLessonsController(ITeacherLessonService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("school/{schoolId}")]
    public async Task<IActionResult> GetBySchool(string schoolId)
        => Ok(await _service.GetBySchoolIdAsync(schoolId));

    [HttpGet("teacher/{teacherId}")]
    public async Task<IActionResult> GetByTeacher(string teacherId)
        => Ok(await _service.GetByTeacherIdAsync(teacherId));

    [HttpPost]
    public async Task<IActionResult> Create(CreateTeacherLessonDto dto)
    {
        await _service.CreateAsync(dto);
        return Ok("Atama yapıldı.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _service.DeleteAsync(id);
        return Ok("Silindi.");
    }
}