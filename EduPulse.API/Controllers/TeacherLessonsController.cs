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
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("school/{schoolId}")]
    public async Task<IActionResult> GetBySchool(string schoolId)
    {
        var result = await _service.GetBySchoolIdAsync(schoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("teacher/{teacherId}")]
    public async Task<IActionResult> GetByTeacher(string teacherId)
    {
        var result = await _service.GetByTeacherIdAsync(teacherId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTeacherLessonDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateTeacherLessonDto dto)
    {
        var result = await _service.UpdateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _service.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}