using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Schools;
using Microsoft.AspNetCore.Mvc;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SchoolsController : ControllerBase
{
    private readonly ISchoolService _schoolService;

    public SchoolsController(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var schools = await _schoolService.GetAllAsync();
        return Ok(schools);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var school = await _schoolService.GetByIdAsync(id);

        if (school is null)
            return NotFound("Okul bulunamadı.");

        return Ok(school);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSchoolDto dto)
    {
        await _schoolService.CreateAsync(dto);
        return Ok("Okul başarıyla oluşturuldu.");
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateSchoolDto dto)
    {
        await _schoolService.UpdateAsync(dto);
        return Ok("Okul başarıyla güncellendi.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _schoolService.DeleteAsync(id);
        return Ok("Okul başarıyla silindi.");
    }
}