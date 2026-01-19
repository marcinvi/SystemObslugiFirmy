using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReklamacjeAPI.DTOs;

namespace ReklamacjeAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/messages")]
public class MessagesController : ControllerBase
{
    [HttpGet]
    public ActionResult<ApiResponse<List<MessageDto>>> GetMessages([FromQuery] int? returnId = null)
    {
        return StatusCode(501, ApiResponse<List<MessageDto>>.ErrorResponse(
            "Messages list endpoint not implemented yet."));
    }

    [HttpPost]
    public ActionResult<ApiResponse<MessageDto>> CreateMessage([FromBody] MessageCreateRequest request)
    {
        return StatusCode(501, ApiResponse<MessageDto>.ErrorResponse(
            "Create message endpoint not implemented yet."));
    }

    [HttpPatch("{id:int}/read")]
    public ActionResult<ApiResponse<object>> MarkRead(int id)
    {
        return StatusCode(501, ApiResponse<object>.ErrorResponse(
            "Mark-read endpoint not implemented yet."));
    }
}
