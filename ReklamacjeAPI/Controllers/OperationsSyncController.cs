using Microsoft.AspNetCore.Mvc;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Services;

namespace ReklamacjeAPI.Controllers;

[ApiController]
[Route("api/ops-sync")]
public class OperationsSyncController : ControllerBase
{
    private readonly DpdSyncCoordinatorService _dpd;
    private readonly GoogleSyncCoordinatorService _google;

    public OperationsSyncController(
        DpdSyncCoordinatorService dpd,
        GoogleSyncCoordinatorService google)
    {
        _dpd = dpd;
        _google = google;
    }

    [HttpGet("status")]
    public ActionResult<ApiResponse<OperationsSyncSnapshotDto>> Status()
    {
        var snapshot = new OperationsSyncSnapshotDto
        {
            Dpd = _dpd.GetStatusSnapshot(),
            Google = _google.GetStatusSnapshot()
        };

        return Ok(ApiResponse<OperationsSyncSnapshotDto>.SuccessResponse(snapshot));
    }

    [HttpPost("trigger")]
    public async Task<ActionResult<ApiResponse<OperationsSyncSnapshotDto>>> Trigger()
    {
        var dpdStatus = await _dpd.TriggerSyncAsync("manual");
        var googleStatus = await _google.TriggerSyncAsync("manual");

        var snapshot = new OperationsSyncSnapshotDto
        {
            Dpd = dpdStatus,
            Google = googleStatus
        };

        return Ok(ApiResponse<OperationsSyncSnapshotDto>.SuccessResponse(snapshot, "DPD/Google sync wykonany."));
    }
}
