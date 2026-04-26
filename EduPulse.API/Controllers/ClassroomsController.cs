using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Classrooms;
using Microsoft.AspNetCore.Mvc;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClassroomsController : ControllerBase
{
    private readonly IClassroomService _classroomService;

    public ClassroomsController(IClassroomService classroomService)
    {
        _classroomService = classroomService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var classrooms = await _classroomService.GetAllAsync();
        return Ok(classrooms);
    }

    [HttpGet("school/{schoolId}")]
    public async Task<IActionResult> GetBySchoolId(string schoolId)
    {
        var classrooms = await _classroomService.GetBySchoolIdAsync(schoolId);
        return Ok(classrooms);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var classroom = await _classroomService.GetByIdAsync(id);

        if (classroom is null)
            return NotFound("Sınıf bulunamadı.");

        return Ok(classroom);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClassroomDto dto)
    {
        await _classroomService.CreateAsync(dto);
        return Ok("Sınıf başarıyla oluşturuldu.");
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateClassroomDto dto)
    {
        await _classroomService.UpdateAsync(dto);
        return Ok("Sınıf başarıyla güncellendi.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _classroomService.DeleteAsync(id);
        return Ok("Sınıf başarıyla silindi.");
    }
}