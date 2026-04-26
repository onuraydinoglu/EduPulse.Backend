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
        return Ok(await _classroomService.GetAllAsync());
    }

    [HttpGet("school/{schoolId}")]
    public async Task<IActionResult> GetBySchool(string schoolId)
    {
        return Ok(await _classroomService.GetBySchoolIdAsync(schoolId));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClassroomDto dto)
    {
        await _classroomService.CreateAsync(dto);
        return Ok("Sınıf oluşturuldu.");
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateClassroomDto dto)
    {
        await _classroomService.UpdateAsync(dto);
        return Ok("Sınıf güncellendi.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _classroomService.DeleteAsync(id);
        return Ok("Sınıf silindi.");
    }
}