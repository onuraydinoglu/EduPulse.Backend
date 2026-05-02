using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Clubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ClubsController : ControllerBase
{
    private readonly IClubService _clubService;

    public ClubsController(IClubService clubService)
    {
        _clubService = clubService;
    }

    private string? RoleName => User.FindFirst(ClaimTypes.Role)?.Value;
    private string? SchoolId => User.FindFirst("schoolId")?.Value;

    [HttpGet]
    [Authorize(Roles = "schooladmin,teacher")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _clubService.GetAllForCurrentUserAsync(RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "schooladmin,teacher")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _clubService.GetByIdForCurrentUserAsync(id, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> Create(CreateClubDto dto)
    {
        var result = await _clubService.CreateAsync(dto, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> Update(UpdateClubDto dto)
    {
        var result = await _clubService.UpdateAsync(dto, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "schooladmin")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _clubService.DeleteAsync(id, RoleName, SchoolId);
        return StatusCode(result.StatusCode, result);
    }
}