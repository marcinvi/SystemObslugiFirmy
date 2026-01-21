using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Services;

namespace ReklamacjeAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/messages")]
public class MessagesController : ControllerBase
{
    private readonly MessagesService _messagesService;

    public MessagesController(MessagesService messagesService)
    {
        _messagesService = messagesService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<MessageDto>>>> GetMessages([FromQuery] int? returnId = null)
    {
        var messages = await _messagesService.GetMessagesAsync(returnId);
        return Ok(ApiResponse<List<MessageDto>>.SuccessResponse(messages));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MessageDto>>> CreateMessage([FromBody] MessageCreateRequest request)
    {
        var message = await _messagesService.CreateMessageAsync(request);
        if (message == null)
        {
            return BadRequest(ApiResponse<MessageDto>.ErrorResponse("Nie udało się utworzyć wiadomości."));
        }

        return Ok(ApiResponse<MessageDto>.SuccessResponse(message));
    }

    [HttpPatch("{id:int}/read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkRead(int id)
    {
        var success = await _messagesService.MarkReadAsync(id);
        if (!success)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Wiadomość nie znaleziona."));
        }

        return Ok(ApiResponse<object>.SuccessResponse(new { id }));
    }
}
