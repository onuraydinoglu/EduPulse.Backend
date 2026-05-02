using EduPulse.Business.Abstracts;
using EduPulse.DTOs.ClubMembers;
using Microsoft.AspNetCore.Mvc;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClubMembersController : ControllerBase
{
    private readonly IClubMemberService _clubMemberService;

    public ClubMembersController(IClubMemberService clubMemberService)
    {
        _clubMemberService = clubMemberService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roleName = User.FindFirst("roleName")?.Value;
        var schoolId = User.FindFirst("schoolId")?.Value;

        var result = await _clubMemberService.GetAllForCurrentUserAsync(roleName, schoolId);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [HttpGet("club/{clubId}")]
    public async Task<IActionResult> GetByClubId(string clubId)
    {
        var roleName = User.FindFirst("roleName")?.Value;
        var schoolId = User.FindFirst("schoolId")?.Value;

        var result = await _clubMemberService.GetByClubIdForCurrentUserAsync(
            clubId,
            roleName,
            schoolId
        );

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetByStudentId(string studentId)
    {
        var roleName = User.FindFirst("roleName")?.Value;
        var schoolId = User.FindFirst("schoolId")?.Value;

        var result = await _clubMemberService.GetByStudentIdForCurrentUserAsync(
            studentId,
            roleName,
            schoolId
        );

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClubMemberDto dto)
    {
        var roleName = User.FindFirst("roleName")?.Value;
        var schoolId = User.FindFirst("schoolId")?.Value;

        var result = await _clubMemberService.CreateAsync(dto, roleName, schoolId);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var roleName = User.FindFirst("roleName")?.Value;
        var schoolId = User.FindFirst("schoolId")?.Value;

        var result = await _clubMemberService.DeleteAsync(id, roleName, schoolId);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }
}