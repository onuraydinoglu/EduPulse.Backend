using EduPulse.Business.Abstracts;
using EduPulse.DTOs.StudentGrades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StudentGradesController : ControllerBase
{
    private readonly IStudentGradeService _studentGradeService;

    public StudentGradesController(IStudentGradeService studentGradeService)
    {
        _studentGradeService = studentGradeService;
    }

    private string? GetSchoolId()
    {
        return User.FindFirst("schoolId")?.Value;
    }

    private string? GetTeacherId()
    {
        return User.FindFirst("teacherId")?.Value;
    }

    private string? GetRole()
    {
        return User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
    }

    [HttpGet]
    [Authorize(Roles = "SchoolAdmin,Teacher")]
    public async Task<IActionResult> GetAll()
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        var role = GetRole();

        if (role == "Teacher")
        {
            var teacherId = GetTeacherId();

            if (string.IsNullOrWhiteSpace(teacherId))
                return Unauthorized("Öğretmen bilgisi token içinde bulunamadı.");

            var teacherGrades = await _studentGradeService.GetByTeacherIdAsync(teacherId);
            return StatusCode(teacherGrades.StatusCode, teacherGrades);
        }

        var result = await _studentGradeService.GetBySchoolIdAsync(schoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "SchoolAdmin,Teacher")]
    public async Task<IActionResult> GetById(string id)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        var result = await _studentGradeService.GetByIdAsync(id);

        if (result.Data == null || result.Data.SchoolId != schoolId)
            return Forbid("Bu kayda erişim yetkiniz yok.");

        var role = GetRole();

        if (role == "Teacher")
        {
            var teacherId = GetTeacherId();

            if (string.IsNullOrWhiteSpace(teacherId))
                return Unauthorized("Öğretmen bilgisi token içinde bulunamadı.");

            if (result.Data.TeacherId != teacherId)
                return Forbid("Bu nota erişim yetkiniz yok.");
        }

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("student/{studentId}")]
    [Authorize(Roles = "SchoolAdmin,Teacher")]
    public async Task<IActionResult> GetByStudentId(string studentId)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        var result = await _studentGradeService.GetByStudentIdAsync(studentId);

        var filteredData = result.Data?
            .Where(x => x.SchoolId == schoolId)
            .ToList();

        result.Data = filteredData ?? new List<StudentGradeListDto>();

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("lesson/{lessonId}")]
    [Authorize(Roles = "SchoolAdmin,Teacher")]
    public async Task<IActionResult> GetByLessonId(string lessonId)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        var result = await _studentGradeService.GetByLessonIdAsync(lessonId);

        var filteredData = result.Data?
            .Where(x => x.SchoolId == schoolId)
            .ToList();

        result.Data = filteredData ?? new List<StudentGradeListDto>();

        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Create(CreateStudentGradeDto dto)
    {
        var schoolId = GetSchoolId();
        var teacherId = GetTeacherId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        if (string.IsNullOrWhiteSpace(teacherId))
            return Unauthorized("Öğretmen bilgisi token içinde bulunamadı.");

        dto.SchoolId = schoolId;
        dto.TeacherId = teacherId;

        var result = await _studentGradeService.CreateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Update(UpdateStudentGradeDto dto)
    {
        var schoolId = GetSchoolId();
        var teacherId = GetTeacherId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        if (string.IsNullOrWhiteSpace(teacherId))
            return Unauthorized("Öğretmen bilgisi token içinde bulunamadı.");

        var existingGrade = await _studentGradeService.GetByIdAsync(dto.Id);

        if (existingGrade.Data == null || existingGrade.Data.SchoolId != schoolId)
            return Forbid("Bu kaydı güncelleme yetkiniz yok.");

        if (existingGrade.Data.TeacherId != teacherId)
            return Forbid("Bu notu sadece ekleyen öğretmen güncelleyebilir.");

        dto.SchoolId = schoolId;
        dto.TeacherId = teacherId;

        var result = await _studentGradeService.UpdateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Delete(string id)
    {
        var schoolId = GetSchoolId();
        var teacherId = GetTeacherId();

        if (string.IsNullOrWhiteSpace(schoolId))
            return Unauthorized("Okul bilgisi token içinde bulunamadı.");

        if (string.IsNullOrWhiteSpace(teacherId))
            return Unauthorized("Öğretmen bilgisi token içinde bulunamadı.");

        var grade = await _studentGradeService.GetByIdAsync(id);

        if (grade.Data == null || grade.Data.SchoolId != schoolId)
            return Forbid("Bu kaydı silme yetkiniz yok.");

        if (grade.Data.TeacherId != teacherId)
            return Forbid("Bu notu sadece ekleyen öğretmen silebilir.");

        var result = await _studentGradeService.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}