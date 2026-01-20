using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Services;
using System.Linq;

namespace ReklamacjeAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/returns")]
public class ReturnsController : ControllerBase
{
    private readonly ReturnsService _returnsService;

    public ReturnsController(ReturnsService returnsService)
    {
        _returnsService = returnsService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ReturnListItemDto>>>> GetReturns(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? statusWewnetrzny = null,
        [FromQuery] string? statusAllegro = null,
        [FromQuery] int? handlowiecId = null,
        [FromQuery] string? search = null)
    {
        var data = await _returnsService.GetReturnsAsync(page, pageSize, statusWewnetrzny, statusAllegro, handlowiecId, search);
        return Ok(ApiResponse<PaginatedResponse<ReturnListItemDto>>.SuccessResponse(data));
    }

    [HttpGet("assigned")]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ReturnListItemDto>>>> GetAssignedReturns(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? statusWewnetrzny = null,
        [FromQuery] string? statusAllegro = null,
        [FromQuery] string? search = null)
    {
        var login = Request.Headers["X-User"].FirstOrDefault();
        var userId = await _returnsService.GetUserIdByLoginAsync(login ?? string.Empty);
        if (!userId.HasValue)
        {
            return Ok(ApiResponse<PaginatedResponse<ReturnListItemDto>>.SuccessResponse(
                new PaginatedResponse<ReturnListItemDto> { Page = page, PageSize = pageSize, TotalItems = 0 }));
        }

        var data = await _returnsService.GetReturnsAsync(page, pageSize, statusWewnetrzny, statusAllegro, userId, search);
        return Ok(ApiResponse<PaginatedResponse<ReturnListItemDto>>.SuccessResponse(data));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ReturnDetailsDto>>> GetReturn(int id)
    {
        var data = await _returnsService.GetReturnDetailsAsync(id);
        if (data == null)
        {
            return NotFound(ApiResponse<ReturnDetailsDto>.ErrorResponse("Return not found."));
        }
        return Ok(ApiResponse<ReturnDetailsDto>.SuccessResponse(data));
    }

    [HttpPatch("{id:int}/warehouse")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateWarehouse(int id, [FromBody] ReturnWarehouseUpdateRequest request)
    {
        var success = await _returnsService.UpdateWarehouseAsync(id, request);
        if (!success)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Return not found."));
        }
        return Ok(ApiResponse<object>.SuccessResponse(new { id }));
    }

    [HttpPost("{id:int}/forward-to-sales")]
    public ActionResult<ApiResponse<object>> ForwardToSales(int id, [FromBody] ReturnForwardToSalesRequest request)
    {
        return StatusCode(501, ApiResponse<object>.ErrorResponse(
            "Forward-to-sales endpoint not implemented yet."));
    }

    [HttpPatch("{id:int}/decision")]
    public async Task<ActionResult<ApiResponse<ReturnDecisionResponse>>> SaveDecision(int id, [FromBody] ReturnDecisionRequest request)
    {
        var response = await _returnsService.SaveDecisionAsync(id, request);
        if (response == null)
        {
            return BadRequest(ApiResponse<ReturnDecisionResponse>.ErrorResponse("Nie udało się zapisać decyzji."));
        }
        return Ok(ApiResponse<ReturnDecisionResponse>.SuccessResponse(response));
    }

    [HttpPost("manual")]
    public ActionResult<ApiResponse<object>> CreateManualReturn([FromBody] ReturnManualCreateRequest request)
    {
        return StatusCode(501, ApiResponse<object>.ErrorResponse(
            "Manual return creation endpoint not implemented yet."));
    }

    [HttpPatch("{id:int}/archive")]
    public ActionResult<ApiResponse<object>> ArchiveReturn(int id)
    {
        return StatusCode(501, ApiResponse<object>.ErrorResponse(
            "Archive endpoint not implemented yet."));
    }

    [HttpGet("{id:int}/actions")]
    public ActionResult<ApiResponse<List<ReturnActionDto>>> GetActions(int id)
    {
        return StatusCode(501, ApiResponse<List<ReturnActionDto>>.ErrorResponse(
            "Return actions endpoint not implemented yet."));
    }

    [HttpPost("{id:int}/actions")]
    public ActionResult<ApiResponse<ReturnActionDto>> AddAction(int id, [FromBody] ReturnActionCreateRequest request)
    {
        return StatusCode(501, ApiResponse<ReturnActionDto>.ErrorResponse(
            "Add action endpoint not implemented yet."));
    }

    [HttpGet("summary")]
    public ActionResult<ApiResponse<ReturnSummaryResponse>> GetSummary(
        [FromQuery] int? handlowiecId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        return StatusCode(501, ApiResponse<ReturnSummaryResponse>.ErrorResponse(
            "Summary endpoint not implemented yet."));
    }

    [HttpGet("summary/export")]
    public ActionResult ExportSummary(
        [FromQuery] int? handlowiecId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        return StatusCode(501, ApiResponse<object>.ErrorResponse(
            "Summary export endpoint not implemented yet."));
    }

    [HttpPost("{id:int}/forward-to-complaints")]
    public ActionResult<ApiResponse<object>> ForwardToComplaints(int id, [FromBody] ForwardToComplaintRequest request)
    {
        return StatusCode(501, ApiResponse<object>.ErrorResponse(
            "Forward-to-complaints endpoint not implemented yet."));
    }
}
