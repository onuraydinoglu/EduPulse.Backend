using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    private string? RoleName => User.FindFirst(ClaimTypes.Role)?.Value;
    private string? SchoolId => User.FindFirst("schoolId")?.Value;

    [HttpGet]
    [Authorize(Roles = "schooladmin,officer")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _eventService.GetAllForCurrentUserAsync(RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "schooladmin,officer")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _eventService.GetByIdForCurrentUserAsync(id, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Roles = "schooladmin,officer")]
    public async Task<IActionResult> Create(CreateEventDto dto)
    {
        var result = await _eventService.CreateAsync(dto, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    [Authorize(Roles = "schooladmin,officer")]
    public async Task<IActionResult> Update(UpdateEventDto dto)
    {
        var result = await _eventService.UpdateAsync(dto, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "schooladmin,officer")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _eventService.DeleteAsync(id, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }
}