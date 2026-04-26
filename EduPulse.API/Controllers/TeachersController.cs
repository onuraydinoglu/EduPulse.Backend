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
        var result = await _teacherService.GetAllAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("school/{schoolId}")]
    public async Task<IActionResult> GetBySchoolId(string schoolId)
    {
        var result = await _teacherService.GetBySchoolIdAsync(schoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _teacherService.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTeacherDto dto)
    {
        var result = await _teacherService.CreateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateTeacherDto dto)
    {
        var result = await _teacherService.UpdateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _teacherService.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}