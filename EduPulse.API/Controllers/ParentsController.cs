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
    {
        var result = await _parentService.GetAllAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("school/{schoolId}")]
    public async Task<IActionResult> GetBySchool(string schoolId)
    {
        var result = await _parentService.GetBySchoolIdAsync(schoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _parentService.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateParentDto dto)
    {
        var result = await _parentService.CreateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateParentDto dto)
    {
        var result = await _parentService.UpdateAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _parentService.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}