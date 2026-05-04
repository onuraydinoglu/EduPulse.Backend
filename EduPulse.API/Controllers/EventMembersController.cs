using EduPulse.Business.Abstracts;
using EduPulse.DTOs.EventMembers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "schooladmin,teacher,officer")]
public class EventMembersController : ControllerBase
{
    private readonly IEventMemberService _eventMemberService;

    public EventMembersController(IEventMemberService eventMemberService)
    {
        _eventMemberService = eventMemberService;
    }

    private string? RoleName => User.FindFirst(ClaimTypes.Role)?.Value;
    private string? SchoolId => User.FindFirst("schoolId")?.Value;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _eventMemberService.GetAllForCurrentUserAsync(RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> GetByEventId(string eventId)
    {
        var result = await _eventMemberService.GetByEventIdForCurrentUserAsync(eventId, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetByStudentId(string studentId)
    {
        var result = await _eventMemberService.GetByStudentIdForCurrentUserAsync(studentId, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Roles = "schooladmin,officer")]
    public async Task<IActionResult> Create(CreateEventMemberDto dto)
    {
        var result = await _eventMemberService.CreateAsync(dto, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("payment")]
    [Authorize(Roles = "schooladmin,officer")]
    public async Task<IActionResult> UpdatePayment(UpdateEventMemberPaymentDto dto)
    {
        var result = await _eventMemberService.UpdatePaymentAsync(dto, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "schooladmin,officer")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _eventMemberService.DeleteAsync(id, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }
}