using EduPulse.Business.Abstracts;
using EduPulse.DTOs.StudentGrades;
using Microsoft.AspNetCore.Mvc;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentGradesController : ControllerBase
{
    private readonly IStudentGradeService _service;

    public StudentGradesController(IStudentGradeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetByStudent(string studentId)
        => Ok(await _service.GetByStudentIdAsync(studentId));

    [HttpGet("lesson/{lessonId}")]
    public async Task<IActionResult> GetByLesson(string lessonId)
        => Ok(await _service.GetByLessonIdAsync(lessonId));

    [HttpPost]
    public async Task<IActionResult> Create(CreateStudentGradeDto dto)
    {
        await _service.CreateAsync(dto);
        return Ok("Not eklendi.");
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateStudentGradeDto dto)
    {
        await _service.UpdateAsync(dto);
        return Ok("Not güncellendi.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _service.DeleteAsync(id);
        return Ok("Silindi.");
    }
}