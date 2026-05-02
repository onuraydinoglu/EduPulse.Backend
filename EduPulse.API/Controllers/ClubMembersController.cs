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
    private readonly IClubMemberService _clubMemberService;

    public ClubMembersController(IClubMemberService clubMemberService)
    {
        _clubMemberService = clubMemberService;
    }

    private string? RoleName => User.FindFirst(ClaimTypes.Role)?.Value;
    private string? SchoolId => User.FindFirst("schoolId")?.Value;

    [HttpGet]
    [Authorize(Roles = "schooladmin,teacher")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _clubMemberService.GetAllForCurrentUserAsync(RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("club/{clubId}")]
    [Authorize(Roles = "schooladmin,teacher")]
    public async Task<IActionResult> GetByClubId(string clubId)
    {
        var result = await _clubMemberService.GetByClubIdForCurrentUserAsync(
            clubId,
            RoleName,
            SchoolId
        );

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("student/{studentId}")]
    [Authorize(Roles = "schooladmin,teacher")]
    public async Task<IActionResult> GetByStudentId(string studentId)
    {
        var result = await _clubMemberService.GetByStudentIdForCurrentUserAsync(
            studentId,
            RoleName,
            SchoolId
        );

        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> Create(CreateClubMemberDto dto)
    {
        var result = await _clubMemberService.CreateAsync(dto, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _clubMemberService.DeleteAsync(id, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }
}