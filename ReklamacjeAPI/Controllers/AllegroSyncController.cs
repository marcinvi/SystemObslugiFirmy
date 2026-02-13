using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Services;

namespace ReklamacjeAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/allegro-sync")]
public class AllegroSyncController : ControllerBase
{
    private readonly AllegroSyncCoordinatorService _coordinator;

    public AllegroSyncController(AllegroSyncCoordinatorService coordinator)
    {
        _coordinator = coordinator;
    }

    [HttpGet("status")]
    public ActionResult<ApiResponse<AllegroSyncStatusDto>> GetStatus()
    {
        var status = _coordinator.GetStatusSnapshot();
        return Ok(ApiResponse<AllegroSyncStatusDto>.SuccessResponse(status));
    }

    [HttpPost("trigger")]
    public async Task<ActionResult<ApiResponse<AllegroSyncRunResultDto>>> Trigger()
    {
        var result = await _coordinator.TriggerSyncAsync("manual");
        if (!result.Success)
        {
            return StatusCode(500, ApiResponse<AllegroSyncRunResultDto>.ErrorResponse(result.Message));
        }

        return Ok(ApiResponse<AllegroSyncRunResultDto>.SuccessResponse(result, result.Message));
    }
}
