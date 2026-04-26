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
        var result = await _classroomService.GetAllAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("school/{schoolId}")]
    public async Task<IActionResult> GetBySchoolId(string schoolId)
    {
        var result = await _classroomService.GetBySchoolIdAsync(schoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _classroomService.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClassroomDto dto)
    {
        var result = await _classroomService.CreateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateClassroomDto dto)
    {
        var result = await _classroomService.UpdateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _classroomService.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}