using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Services;
using System.Linq;

namespace ReklamacjeAPI.Controllers;

[ApiController]
[Route("api/returns")]
public class ReturnsController : ControllerBase
{
    private readonly ReturnsService _returnsService;
    private readonly ReturnSyncProgressService _progressService;
    private readonly ILogger<ReturnsController> _logger;

    public ReturnsController(ReturnsService returnsService, ReturnSyncProgressService progressService, ILogger<ReturnsController> logger)
    {
        _returnsService = returnsService;
        _progressService = progressService;
        _logger = logger;
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
        var userId = GetUserIdFromClaims();
        if (!userId.HasValue)
        {
            var login = Request.Headers["X-User"].FirstOrDefault() ?? User.Identity?.Name;
            userId = await _returnsService.GetUserIdByLoginAsync(login ?? string.Empty);
        }

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

    [HttpGet("lookup")]
    public async Task<ActionResult<ApiResponse<ReturnDetailsDto>>> GetReturnByCode([FromQuery] string code)
    {
        var data = await _returnsService.GetReturnByCodeAsync(code);
        if (data == null)
        {
            return NotFound(ApiResponse<ReturnDetailsDto>.ErrorResponse("Return not found."));
        }

        return Ok(ApiResponse<ReturnDetailsDto>.SuccessResponse(data));
    }

    [HttpPost("sync")]
    public async Task<ActionResult<ApiResponse<ReturnSyncResponse>>> SyncReturns([FromBody] ReturnSyncRequest? request)
    {
        var result = await _returnsService.SyncReturnsFromAllegroAsync(request, GetUserDisplayName());
        return Ok(ApiResponse<ReturnSyncResponse>.SuccessResponse(result));
    }

    [HttpPost("sync/start")]
    public ActionResult<ApiResponse<ReturnSyncJobResponse>> StartSync([FromBody] ReturnSyncRequest? request)
    {
        var userDisplayName = GetUserDisplayName();
        var job = _progressService.StartJob(userDisplayName);

        _ = Task.Run(async () =>
        {
            try
            {
                await _returnsService.SyncReturnsFromAllegroInternalAsync(request, userDisplayName, job);
            }
            catch (Exception ex)
            {
                _progressService.Fail(job, ex.Message);
                _logger.LogError(ex, "Synchronizacja zwrotw zakoczona bdem. JobId={JobId}", job.JobId);
            }
        });

        var response = new ReturnSyncJobResponse
        {
            JobId = job.JobId,
            StartedAt = job.StartedAt,
            Status = job.Status.ToString()
        };

        return Accepted(ApiResponse<ReturnSyncJobResponse>.SuccessResponse(response));
    }

    [HttpGet("sync/status/{jobId}")]
    public ActionResult<ApiResponse<ReturnSyncProgress>> GetSyncStatus(string jobId)
    {
        var job = _progressService.GetJob(jobId);
        if (job == null)
        {
            return NotFound(ApiResponse<ReturnSyncProgress>.ErrorResponse("Nie znaleziono zadania synchronizacji."));
        }

        return Ok(ApiResponse<ReturnSyncProgress>.SuccessResponse(job));
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
    public async Task<ActionResult<ApiResponse<object>>> ForwardToSales(int id, [FromBody] ReturnForwardToSalesRequest request)
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

        var success = await _returnsService.ForwardToSalesAsync(id, request, userId.Value, GetUserDisplayName());
        if (!success)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Nie udało się przekazać zwrotu do handlowca."));
        }

        return Ok(ApiResponse<object>.SuccessResponse(new { id }));
    }

    [HttpPatch("{id:int}/decision")]
    public async Task<ActionResult<ApiResponse<ReturnDecisionResponse>>> SaveDecision(int id, [FromBody] ReturnDecisionRequest request)
    {
        var response = await _returnsService.SaveDecisionAsync(id, request, GetUserDisplayName());
        if (response == null)
        {
            return BadRequest(ApiResponse<ReturnDecisionResponse>.ErrorResponse("Nie udało się zapisać decyzji."));
        }
        return Ok(ApiResponse<ReturnDecisionResponse>.SuccessResponse(response));
    }

    [HttpPost("{id:int}/forward-to-warehouse")]
    public async Task<ActionResult<ApiResponse<object>>> ForwardToWarehouse(int id, [FromBody] ReturnForwardToWarehouseRequest request)
    {
        var updated = await _returnsService.ForwardToWarehouseAsync(id, request, GetUserDisplayName());
        return Ok(ApiResponse<object>.SuccessResponse(new { id, statusUpdated = updated }));
    }

    [HttpGet("{id:int}/refund-context")]
    public async Task<ActionResult<ApiResponse<ReturnRefundContextDto>>> GetRefundContext(int id)
    {
        var context = await _returnsService.GetRefundContextAsync(id);
        if (context == null)
        {
            return BadRequest(ApiResponse<ReturnRefundContextDto>.ErrorResponse("Nie udało się pobrać danych do zwrotu wpłaty."));
        }

        return Ok(ApiResponse<ReturnRefundContextDto>.SuccessResponse(context));
    }

    [HttpPost("{id:int}/reject")]
    public async Task<ActionResult<ApiResponse<object>>> RejectReturn(int id, [FromBody] RejectCustomerReturnRequestDto request)
    {
        if (request?.Rejection == null || string.IsNullOrWhiteSpace(request.Rejection.Code))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Brak powodu odrzucenia."));
        }

        var success = await _returnsService.RejectReturnAsync(id, request, GetUserDisplayName());
        if (!success)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Nie udało się odrzucić zwrotu."));
        }

        return Ok(ApiResponse<object>.SuccessResponse(new { id }));
    }

    [HttpPost("{id:int}/refund")]
    public async Task<ActionResult<ApiResponse<object>>> RefundPayment(int id, [FromBody] PaymentRefundRequestDto request)
    {
        if (request?.Payment == null || string.IsNullOrWhiteSpace(request.Payment.Id))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Brak identyfikatora płatności."));
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Brak powodu zwrotu."));
        }

        var success = await _returnsService.RefundPaymentAsync(id, request, GetUserDisplayName());
        if (!success)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Nie udało się zlecić zwrotu wpłaty."));
        }

        return Ok(ApiResponse<object>.SuccessResponse(new { id }));
    }

    [HttpGet("manual/meta")]
    public async Task<ActionResult<ApiResponse<ReturnManualMetaDto>>> GetManualReturnMeta()
    {
        var data = await _returnsService.GetManualReturnMetaAsync();
        return Ok(ApiResponse<ReturnManualMetaDto>.SuccessResponse(data));
    }

    [HttpPost("manual")]
    public async Task<ActionResult<ApiResponse<object>>> CreateManualReturn([FromBody] ReturnManualCreateRequest request)
    {
        if (request == null)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Brak danych zwrotu ręcznego."));
        }
        if (string.IsNullOrWhiteSpace(request.NumerListu))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Numer listu przewozowego jest wymagany."));
        }
        if (string.IsNullOrWhiteSpace(request.BuyerFullName))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Imię i nazwisko nadawcy jest wymagane."));
        }
        if (request.StanProduktuId <= 0)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Stan produktu jest wymagany."));
        }
        if (request.WybraniHandlowcy == null || request.WybraniHandlowcy.Count == 0)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Brak wybranych handlowców."));
        }

        var userId = GetUserIdFromClaims();
        var userDisplayName = GetUserDisplayName();
        if (!userId.HasValue)
        {
            var login = Request.Headers["X-User"].FirstOrDefault() ?? User.Identity?.Name;
            userId = await _returnsService.GetUserIdByLoginAsync(login ?? string.Empty);
            if (userId.HasValue)
            {
                userDisplayName = await _returnsService.GetUserDisplayNameByIdAsync(userId.Value);
            }
        }
        if (!userId.HasValue)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Brak informacji o użytkowniku."));
        }
        var newId = await _returnsService.CreateManualReturnAsync(request, userId.Value, userDisplayName);
        if (!newId.HasValue)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Nie udało się utworzyć zwrotu ręcznego."));
        }

        return Ok(ApiResponse<object>.SuccessResponse(new { id = newId.Value }));
    }

    [HttpPatch("{id:int}/archive")]
    public async Task<ActionResult<ApiResponse<object>>> ArchiveReturn(int id)
    {
        var success = await _returnsService.ArchiveReturnAsync(id, GetUserDisplayName());
        if (!success)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Return not found."));
        }

        return Ok(ApiResponse<object>.SuccessResponse(new { id }));
    }

    [HttpGet("{id:int}/actions")]
    public async Task<ActionResult<ApiResponse<List<ReturnActionDto>>>> GetActions(int id)
    {
        var actions = await _returnsService.GetActionsAsync(id);
        return Ok(ApiResponse<List<ReturnActionDto>>.SuccessResponse(actions));
    }

    [HttpPost("{id:int}/actions")]
    public async Task<ActionResult<ApiResponse<ReturnActionDto>>> AddAction(int id, [FromBody] ReturnActionCreateRequest request)
    {
        var action = await _returnsService.AddActionAsync(id, GetUserDisplayName(), request);
        if (action == null)
        {
            return NotFound(ApiResponse<ReturnActionDto>.ErrorResponse("Return not found."));
        }

        return Ok(ApiResponse<ReturnActionDto>.SuccessResponse(action));
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<ReturnSummaryResponse>>> GetSummary(
        [FromQuery] int? handlowiecId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        var summary = await _returnsService.GetSummaryAsync(handlowiecId, status, dateFrom, dateTo);
        return Ok(ApiResponse<ReturnSummaryResponse>.SuccessResponse(summary));
    }

    [HttpGet("statuses")]
    public async Task<ActionResult<ApiResponse<List<StatusDto>>>> GetStatuses([FromQuery] string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            return BadRequest(ApiResponse<List<StatusDto>>.ErrorResponse("Brak typu statusu."));
        }

        var data = await _returnsService.GetStatusesAsync(type);
        return Ok(ApiResponse<List<StatusDto>>.SuccessResponse(data));
    }

    [HttpGet("summary/export")]
    public async Task<ActionResult> ExportSummary(
        [FromQuery] int? handlowiecId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        var summary = await _returnsService.GetSummaryAsync(handlowiecId, status, dateFrom, dateTo);

        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("Numer Zwrotu;Produkt;Kto przyjął;Kto podjął decyzję;Jaka decyzja;Uwagi Magazynu;Uwagi Handlowca;Jaki status");

        foreach (var item in summary.Items)
        {
            csvBuilder.AppendLine(string.Join(";", new[]
            {
                item.NumerZwrotu,
                item.Produkt,
                item.KtoPrzyjal,
                item.KtoPodjalDecyzje,
                item.JakaDecyzja,
                item.UwagiMagazynu,
                item.UwagiHandlowca,
                item.Status
            }.Select(value => value.Replace(";", ","))));
        }

        var fileName = $"zwroty_podsumowanie_{DateTime.Now:yyyyMMdd_HHmm}.csv";
        return File(Encoding.UTF8.GetBytes(csvBuilder.ToString()), "text/csv", fileName);
    }

    [HttpPost("{id:int}/forward-to-complaints")]
    public async Task<ActionResult<ApiResponse<object>>> ForwardToComplaints(int id, [FromBody] ForwardToComplaintRequest request)
    {
        if (request == null)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Brak danych przekazania reklamacji."));
        }

        // Jeśli w body przekazujesz ReturnId, musi pasować do {id} z URL
        if (request.ReturnId != 0 && request.ReturnId != id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Niezgodny identyfikator zwrotu."));
        }

        // Ustal userId (claims -> fallback po loginie z nagłówka / Identity)
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

        // (opcjonalnie) jeśli serwis potrzebuje ReturnId w request, możesz go “dopiąć”
        if (request.ReturnId == 0)
            request.ReturnId = id;

        var complaintId = await _returnsService.ForwardToComplaintsAsync(id, request);
        if (!complaintId.HasValue)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Nie udało się przekazać do reklamacji."));
        }

        return Ok(ApiResponse<object>.SuccessResponse(new { id, complaintId }));
    }

    private int? GetUserIdFromClaims()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : null;
    }

    private string GetUserDisplayName()
    {
        return User.FindFirst("DisplayName")?.Value
            ?? User.Identity?.Name
            ?? "System";
    }
}
