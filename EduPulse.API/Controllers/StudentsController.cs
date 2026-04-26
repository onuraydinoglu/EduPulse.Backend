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
        var students = await _studentService.GetAllAsync();
        return Ok(students);
    }

    [HttpGet("school/{schoolId}")]
    public async Task<IActionResult> GetBySchoolId(string schoolId)
    {
        var students = await _studentService.GetBySchoolIdAsync(schoolId);
        return Ok(students);
    }

    [HttpGet("classroom/{classroomId}")]
    public async Task<IActionResult> GetByClassroomId(string classroomId)
    {
        var students = await _studentService.GetByClassroomIdAsync(classroomId);
        return Ok(students);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var student = await _studentService.GetByIdAsync(id);

        if (student is null)
            return NotFound("Öğrenci bulunamadı.");

        return Ok(student);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateStudentDto dto)
    {
        await _studentService.CreateAsync(dto);
        return Ok("Öğrenci başarıyla oluşturuldu.");
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateStudentDto dto)
    {
        await _studentService.UpdateAsync(dto);
        return Ok("Öğrenci başarıyla güncellendi.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _studentService.DeleteAsync(id);
        return Ok("Öğrenci başarıyla silindi.");
    }
}