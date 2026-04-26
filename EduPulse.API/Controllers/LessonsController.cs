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
        var lessons = await _lessonService.GetAllAsync();
        return Ok(lessons);
    }

    [HttpGet("school/{schoolId}")]
    public async Task<IActionResult> GetBySchoolId(string schoolId)
    {
        var lessons = await _lessonService.GetBySchoolIdAsync(schoolId);
        return Ok(lessons);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var lesson = await _lessonService.GetByIdAsync(id);

        if (lesson is null)
            return NotFound("Ders bulunamadı.");

        return Ok(lesson);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateLessonDto dto)
    {
        await _lessonService.CreateAsync(dto);
        return Ok("Ders başarıyla oluşturuldu.");
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateLessonDto dto)
    {
        await _lessonService.UpdateAsync(dto);
        return Ok("Ders başarıyla güncellendi.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _lessonService.DeleteAsync(id);
        return Ok("Ders başarıyla silindi.");
    }
}