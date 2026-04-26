using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Parents;
using Microsoft.AspNetCore.Mvc;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ParentsController : ControllerBase
{
    private readonly IParentService _parentService;

    public ParentsController(IParentService parentService)
    {
        _parentService = parentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _parentService.GetAllAsync());

    [HttpGet("school/{schoolId}")]
    public async Task<IActionResult> GetBySchool(string schoolId)
        => Ok(await _parentService.GetBySchoolIdAsync(schoolId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var parent = await _parentService.GetByIdAsync(id);

        if (parent is null)
            return NotFound("Veli bulunamadı.");

        return Ok(parent);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateParentDto dto)
    {
        await _parentService.CreateAsync(dto);
        return Ok("Veli oluşturuldu.");
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateParentDto dto)
    {
        await _parentService.UpdateAsync(dto);
        return Ok("Veli güncellendi.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _parentService.DeleteAsync(id);
        return Ok("Veli silindi.");
    }
}