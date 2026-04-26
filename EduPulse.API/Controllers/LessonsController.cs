using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Lessons;
using Microsoft.AspNetCore.Mvc;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonsController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _lessonService.GetAllAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("school/{schoolId}")]
    public async Task<IActionResult> GetBySchoolId(string schoolId)
    {
        var result = await _lessonService.GetBySchoolIdAsync(schoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _lessonService.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateLessonDto dto)
    {
        var result = await _lessonService.CreateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateLessonDto dto)
    {
        var result = await _lessonService.UpdateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _lessonService.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}