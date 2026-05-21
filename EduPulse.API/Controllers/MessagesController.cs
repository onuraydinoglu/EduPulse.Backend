using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "schooladmin,teacher,officer,student")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    private string? CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    private string? RoleName => User.FindFirst(ClaimTypes.Role)?.Value;
    private string? SchoolId => User.FindFirst("schoolId")?.Value;

    [HttpGet("users")]
    [Authorize(Roles = "schooladmin,teacher,officer")]
    public async Task<IActionResult> GetMessageUsers()
    {
        var result = await _messageService.GetMessageUsersAsync(
            CurrentUserId,
            RoleName,
            SchoolId
        );

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("inbox")]
    public async Task<IActionResult> GetInbox()
    {
        var result = await _messageService.GetInboxAsync(
            CurrentUserId,
            SchoolId
        );

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("sent")]
    public async Task<IActionResult> GetSent()
    {
        var result = await _messageService.GetSentAsync(
            CurrentUserId,
            SchoolId
        );

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("conversation/{otherUserId}")]
    public async Task<IActionResult> GetConversation(string otherUserId)
    {
        var result = await _messageService.GetConversationAsync(
            otherUserId,
            CurrentUserId,
            SchoolId
        );

        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Roles = "schooladmin,teacher,officer")]
    public async Task<IActionResult> Send(CreateMessageDto dto)
    {
        var result = await _messageService.SendAsync(
            dto,
            CurrentUserId,
            SchoolId
        );

        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(string id)
    {
        var result = await _messageService.MarkAsReadAsync(
            id,
            CurrentUserId,
            SchoolId
        );

        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _messageService.DeleteAsync(
            id,
            CurrentUserId,
            SchoolId
        );

        return StatusCode(result.StatusCode, result);
    }
}