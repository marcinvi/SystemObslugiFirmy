using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Services;

namespace ReklamacjeAPI.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly NotificationsService _notificationsService;
    private readonly ReturnsService _returnsService;

    public NotificationsController(NotificationsService notificationsService, ReturnsService returnsService)
    {
        _notificationsService = notificationsService;
        _returnsService = returnsService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<NotificationDto>>>> GetNotifications([FromQuery] bool? onlyUnread = null)
    {
        var userId = GetUserIdFromClaims();
        if (!userId.HasValue)
        {
            var login = Request.Headers["X-User"].FirstOrDefault() ?? User.Identity?.Name;
            userId = await _returnsService.GetUserIdByLoginAsync(login ?? string.Empty);
        }

        if (!userId.HasValue)
        {
            return BadRequest(ApiResponse<List<NotificationDto>>.ErrorResponse("Brak informacji o użytkowniku."));
        }

        var notifications = await _notificationsService.GetNotificationsAsync(userId.Value, onlyUnread);
        return Ok(ApiResponse<List<NotificationDto>>.SuccessResponse(notifications));
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount()
    {
        var userId = GetUserIdFromClaims();
        if (!userId.HasValue)
        {
            var login = Request.Headers["X-User"].FirstOrDefault() ?? User.Identity?.Name;
            userId = await _returnsService.GetUserIdByLoginAsync(login ?? string.Empty);
        }

        if (!userId.HasValue)
        {
            return BadRequest(ApiResponse<int>.ErrorResponse("Brak informacji o użytkowniku."));
        }

        var count = await _notificationsService.GetUnreadCountAsync(userId.Value);
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }

    [HttpPost("{id}/mark-read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAsRead(int id)
    {
        var userId = GetUserIdFromClaims();
        if (!userId.HasValue)
        {
            var login = Request.Headers["X-User"].FirstOrDefault() ?? User.Identity?.Name;
            userId = await _returnsService.GetUserIdByLoginAsync(login ?? string.Empty);
        }

        if (!userId.HasValue)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Brak informacji o użytkowniku."));
        }

        var success = await _notificationsService.MarkAsReadAsync(id, userId.Value);
        if (!success)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Nie znaleziono powiadomienia."));
        }

        return Ok(ApiResponse<object>.SuccessResponse(new { id }));
    }

    [HttpPost("mark-all-read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAllAsRead()
    {
        var userId = GetUserIdFromClaims();
        if (!userId.HasValue)
        {
            var login = Request.Headers["X-User"].FirstOrDefault() ?? User.Identity?.Name;
            userId = await _returnsService.GetUserIdByLoginAsync(login ?? string.Empty);
        }

        if (!userId.HasValue)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Brak informacji o użytkowniku."));
        }

        var count = await _notificationsService.MarkAllAsReadAsync(userId.Value);
        return Ok(ApiResponse<object>.SuccessResponse(new { markedCount = count }));
    }

    private int? GetUserIdFromClaims()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : null;
    }
}
