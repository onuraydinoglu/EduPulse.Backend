using EduPulse.Business.Abstracts;
using EduPulse.DTOs.StudentGrades;
using Microsoft.AspNetCore.Mvc;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentGradesController : ControllerBase
{
    private readonly IStudentGradeService _studentGradeService;

    public StudentGradesController(IStudentGradeService studentGradeService)
    {
        _studentGradeService = studentGradeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _studentGradeService.GetAllAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetByStudentId(string studentId)
    {
        var result = await _studentGradeService.GetByStudentIdAsync(studentId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("lesson/{lessonId}")]
    public async Task<IActionResult> GetByLessonId(string lessonId)
    {
        var result = await _studentGradeService.GetByLessonIdAsync(lessonId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _studentGradeService.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateStudentGradeDto dto)
    {
        var result = await _studentGradeService.CreateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateStudentGradeDto dto)
    {
        var result = await _studentGradeService.UpdateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _studentGradeService.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}