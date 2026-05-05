using EduPulse.Business.Abstracts;
using EduPulse.DTOs.PersonalNotes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PersonalNotesController : ControllerBase
{
    private readonly IPersonalNoteService _personalNoteService;

    public PersonalNotesController(IPersonalNoteService personalNoteService)
    {
        _personalNoteService = personalNoteService;
    }

    private string? SchoolId => User.FindFirst("schoolId")?.Value;
    private string? UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _personalNoteService.GetAllForCurrentUserAsync(SchoolId, UserId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _personalNoteService.GetByIdForCurrentUserAsync(id, SchoolId, UserId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePersonalNoteDto dto)
    {
        var result = await _personalNoteService.CreateAsync(dto, SchoolId, UserId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdatePersonalNoteDto dto)
    {
        var result = await _personalNoteService.UpdateAsync(dto, SchoolId, UserId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _personalNoteService.DeleteAsync(id, SchoolId, UserId);
        return StatusCode(result.StatusCode, result);
    }
}