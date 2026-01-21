using Microsoft.AspNetCore.Mvc;
using ReklamacjeAPI.DTOs;

namespace ReklamacjeAPI.Controllers;

[ApiController]
[Route("api/warehouse")]
public class WarehouseController : ControllerBase
{
    [HttpGet("search")]
    public ActionResult<ApiResponse<List<WarehouseSearchItemDto>>> Search([FromQuery] string query)
    {
        return StatusCode(501, ApiResponse<List<WarehouseSearchItemDto>>.ErrorResponse(
            "Warehouse search endpoint not implemented yet."));
    }

    [HttpPost("intake")]
    public ActionResult<ApiResponse<object>> Intake([FromBody] WarehouseIntakeRequest request)
    {
        return StatusCode(501, ApiResponse<object>.ErrorResponse(
            "Warehouse intake endpoint not implemented yet."));
    }
}
