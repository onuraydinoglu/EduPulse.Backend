using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Students;
using Microsoft.AspNetCore.Mvc;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _studentService.GetAllAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("school/{schoolId}")]
    public async Task<IActionResult> GetBySchoolId(string schoolId)
    {
        var result = await _studentService.GetBySchoolIdAsync(schoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("classroom/{classroomId}")]
    public async Task<IActionResult> GetByClassroomId(string classroomId)
    {
        var result = await _studentService.GetByClassroomIdAsync(classroomId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _studentService.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateStudentDto dto)
    {
        var result = await _studentService.CreateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateStudentDto dto)
    {
        var result = await _studentService.UpdateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _studentService.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}