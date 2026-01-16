using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReklamacjeAPI.DTOs;

namespace ReklamacjeAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/returns")]
public class ReturnsController : ControllerBase
{
    [HttpGet]
    public ActionResult<ApiResponse<PaginatedResponse<ReturnListItemDto>>> GetReturns(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? statusWewnetrzny = null,
        [FromQuery] string? statusAllegro = null,
        [FromQuery] int? handlowiecId = null,
        [FromQuery] string? search = null)
    {
        return StatusCode(501, ApiResponse<PaginatedResponse<ReturnListItemDto>>.ErrorResponse(
            "Returns list endpoint not implemented yet."));
    }

    [HttpGet("{id:int}")]
    public ActionResult<ApiResponse<ReturnDetailsDto>> GetReturn(int id)
    {
        return StatusCode(501, ApiResponse<ReturnDetailsDto>.ErrorResponse(
            "Return details endpoint not implemented yet."));
    }

    [HttpPatch("{id:int}/warehouse")]
    public ActionResult<ApiResponse<object>> UpdateWarehouse(int id, [FromBody] ReturnWarehouseUpdateRequest request)
    {
        return StatusCode(501, ApiResponse<object>.ErrorResponse(
            "Warehouse update endpoint not implemented yet."));
    }

    [HttpPost("{id:int}/forward-to-sales")]
    public ActionResult<ApiResponse<object>> ForwardToSales(int id, [FromBody] ReturnForwardToSalesRequest request)
    {
        return StatusCode(501, ApiResponse<object>.ErrorResponse(
            "Forward-to-sales endpoint not implemented yet."));
    }

    [HttpPatch("{id:int}/decision")]
    public ActionResult<ApiResponse<ReturnDecisionResponse>> SaveDecision(int id, [FromBody] ReturnDecisionRequest request)
    {
        return StatusCode(501, ApiResponse<ReturnDecisionResponse>.ErrorResponse(
            "Decision endpoint not implemented yet."));
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
