using EduPulse.Business.Abstracts;
using EduPulse.DTOs.StudentGrades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        return User.FindFirst(ClaimTypes.Role)?.Value?.ToLowerInvariant();
    }

    [HttpGet]
    [Authorize(Roles = "schooladmin,teacher")]
    public async Task<IActionResult> GetAll()
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
        {
            return Unauthorized(new
            {
                StatusCode = 401,
                Message = "Okul bilgisi token içinde bulunamadı."
            });
        }

        var role = GetRole();

        if (role == "teacher")
        {
            var teacherId = GetTeacherId();

            if (string.IsNullOrWhiteSpace(teacherId))
            {
                return Unauthorized(new
                {
                    StatusCode = 401,
                    Message = "Öğretmen bilgisi token içinde bulunamadı. Lütfen tekrar giriş yapın."
                });
            }

            var teacherGrades = await _studentGradeService.GetByTeacherIdAsync(teacherId);

            teacherGrades.Data = teacherGrades.Data?
                .Where(x => x.SchoolId == schoolId)
                .ToList() ?? new List<StudentGradeListDto>();

            return StatusCode(teacherGrades.StatusCode, teacherGrades);
        }

        var result = await _studentGradeService.GetBySchoolIdAsync(schoolId);

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "schooladmin,teacher")]
    public async Task<IActionResult> GetById(string id)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
        {
            return Unauthorized(new
            {
                StatusCode = 401,
                Message = "Okul bilgisi token içinde bulunamadı."
            });
        }

        var result = await _studentGradeService.GetByIdAsync(id);

        if (result.Data == null || result.Data.SchoolId != schoolId)
        {
            return Forbid();
        }

        var role = GetRole();

        if (role == "teacher")
        {
            var teacherId = GetTeacherId();

            if (string.IsNullOrWhiteSpace(teacherId))
            {
                return Unauthorized(new
                {
                    StatusCode = 401,
                    Message = "Öğretmen bilgisi token içinde bulunamadı. Lütfen tekrar giriş yapın."
                });
            }

            if (result.Data.TeacherId != teacherId)
            {
                return Forbid();
            }
        }

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("student/{studentId}")]
    [Authorize(Roles = "schooladmin,teacher")]
    public async Task<IActionResult> GetByStudentId(string studentId)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
        {
            return Unauthorized(new
            {
                StatusCode = 401,
                Message = "Okul bilgisi token içinde bulunamadı."
            });
        }

        var result = await _studentGradeService.GetByStudentIdAsync(studentId);

        result.Data = result.Data?
            .Where(x => x.SchoolId == schoolId)
            .ToList() ?? new List<StudentGradeListDto>();

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("lesson/{lessonId}")]
    [Authorize(Roles = "schooladmin,teacher")]
    public async Task<IActionResult> GetByLessonId(string lessonId)
    {
        var schoolId = GetSchoolId();

        if (string.IsNullOrWhiteSpace(schoolId))
        {
            return Unauthorized(new
            {
                StatusCode = 401,
                Message = "Okul bilgisi token içinde bulunamadı."
            });
        }

        var result = await _studentGradeService.GetByLessonIdAsync(lessonId);

        result.Data = result.Data?
            .Where(x => x.SchoolId == schoolId)
            .ToList() ?? new List<StudentGradeListDto>();

        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Roles = "teacher")]
    public async Task<IActionResult> Create(CreateStudentGradeDto dto)
    {
        var schoolId = GetSchoolId();
        var teacherId = GetTeacherId();

        if (string.IsNullOrWhiteSpace(schoolId))
        {
            return Unauthorized(new
            {
                StatusCode = 401,
                Message = "Okul bilgisi token içinde bulunamadı."
            });
        }

        if (string.IsNullOrWhiteSpace(teacherId))
        {
            return Unauthorized(new
            {
                StatusCode = 401,
                Message = "Öğretmen bilgisi token içinde bulunamadı. Lütfen tekrar giriş yapın."
            });
        }

        dto.SchoolId = schoolId;
        dto.TeacherId = teacherId;

        var result = await _studentGradeService.CreateAsync(dto);

        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    [Authorize(Roles = "teacher")]
    public async Task<IActionResult> Update(UpdateStudentGradeDto dto)
    {
        var schoolId = GetSchoolId();
        var teacherId = GetTeacherId();

        if (string.IsNullOrWhiteSpace(schoolId))
        {
            return Unauthorized(new
            {
                StatusCode = 401,
                Message = "Okul bilgisi token içinde bulunamadı."
            });
        }

        if (string.IsNullOrWhiteSpace(teacherId))
        {
            return Unauthorized(new
            {
                StatusCode = 401,
                Message = "Öğretmen bilgisi token içinde bulunamadı. Lütfen tekrar giriş yapın."
            });
        }

        var existingGrade = await _studentGradeService.GetByIdAsync(dto.Id);

        if (existingGrade.Data == null || existingGrade.Data.SchoolId != schoolId)
        {
            return Forbid();
        }

        if (existingGrade.Data.TeacherId != teacherId)
        {
            return Forbid();
        }

        dto.SchoolId = schoolId;
        dto.TeacherId = teacherId;

        var result = await _studentGradeService.UpdateAsync(dto);

        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "teacher")]
    public async Task<IActionResult> Delete(string id)
    {
        var schoolId = GetSchoolId();
        var teacherId = GetTeacherId();

        if (string.IsNullOrWhiteSpace(schoolId))
        {
            return Unauthorized(new
            {
                StatusCode = 401,
                Message = "Okul bilgisi token içinde bulunamadı."
            });
        }

        if (string.IsNullOrWhiteSpace(teacherId))
        {
            return Unauthorized(new
            {
                StatusCode = 401,
                Message = "Öğretmen bilgisi token içinde bulunamadı. Lütfen tekrar giriş yapın."
            });
        }

        var grade = await _studentGradeService.GetByIdAsync(id);

        if (grade.Data == null || grade.Data.SchoolId != schoolId)
        {
            return Forbid();
        }

        if (grade.Data.TeacherId != teacherId)
        {
            return Forbid();
        }

        var result = await _studentGradeService.DeleteAsync(id);

        return StatusCode(result.StatusCode, result);
    }
}