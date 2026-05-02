using EduPulse.Business.Abstracts;
using EduPulse.DTOs.ClubMembers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ClubMembersController : ControllerBase
{
    private readonly IClubService _clubService;

    public ClubMembersController(IClubService clubService)
    {
        _clubService = clubService;
    }

    private string? RoleName => User.FindFirst(ClaimTypes.Role)?.Value;
    private string? SchoolId => User.FindFirst("schoolId")?.Value;

    [HttpGet("club/{clubId}")]
    [Authorize(Roles = "schooladmin,teacher")]
    public async Task<IActionResult> GetMembersByClubId(string clubId)
    {
        var result = await _clubService.GetMembersByClubIdAsync(clubId, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("student/{studentId}/clubs")]
    [Authorize(Roles = "schooladmin,teacher")]
    public async Task<IActionResult> GetClubsByStudentId(string studentId)
    {
        var result = await _clubService.GetClubsByStudentIdAsync(studentId, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> AddMember(AddClubMemberDto dto)
    {
        var result = await _clubService.AddMemberAsync(dto, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("club/{clubId}/student/{studentId}")]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> RemoveMember(string clubId, string studentId)
    {
        var result = await _clubService.RemoveMemberAsync(clubId, studentId, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }
}