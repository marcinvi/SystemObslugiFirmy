using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReklamacjeAPI.Data;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Models;
using ReklamacjeAPI.Services;

namespace ReklamacjeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ZgloszeniaController : ControllerBase
{
    private readonly IZgloszeniaService _zgloszeniaService;
    private readonly ApplicationDbContext _context;

    public ZgloszeniaController(IZgloszeniaService zgloszeniaService, ApplicationDbContext context)
    {
        _zgloszeniaService = zgloszeniaService;
        _context = context;
    }

    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ZgloszenieDto>>>> GetZgloszenia(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest(ApiResponse<PaginatedResponse<ZgloszenieDto>>.ErrorResponse(
                "Nieprawidłowe parametry paginacji"));
        }

        var result = await _zgloszeniaService.GetZgloszeniaAsync(page, pageSize, status, null);
        return Ok(ApiResponse<PaginatedResponse<ZgloszenieDto>>.SuccessResponse(result));
    }

    [HttpGet("moje")]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ZgloszenieDto>>>> GetMojeZgloszenia(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        var userId = GetCurrentUserId();
        var result = await _zgloszeniaService.GetZgloszeniaAsync(page, pageSize, status, userId);
        return Ok(ApiResponse<PaginatedResponse<ZgloszenieDto>>.SuccessResponse(result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ZgloszenieDetailsDto>>> GetZgloszenie(int id)
    {
        var zgloszenie = await _zgloszeniaService.GetZgloszenieByIdAsync(id);

        if (zgloszenie == null)
        {
            return NotFound(ApiResponse<ZgloszenieDetailsDto>.ErrorResponse("Zgłoszenie nie znalezione"));
        }

        return Ok(ApiResponse<ZgloszenieDetailsDto>.SuccessResponse(zgloszenie));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ZgloszenieDto>>> CreateZgloszenie(
        [FromBody] CreateZgloszenieRequest request)
    {
        var userId = GetCurrentUserId();
        var zgloszenie = await _zgloszeniaService.CreateZgloszenieAsync(request, userId);

        return CreatedAtAction(
            nameof(GetZgloszenie),
            new { id = zgloszenie.Id },
            ApiResponse<ZgloszenieDto>.SuccessResponse(zgloszenie, "Zgłoszenie utworzone pomyślnie"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ZgloszenieDto>>> UpdateZgloszenie(
        int id,
        [FromBody] UpdateZgloszenieRequest request)
    {
        var userId = GetCurrentUserId();
        var zgloszenie = await _zgloszeniaService.UpdateZgloszenieAsync(id, request, userId);

        if (zgloszenie == null)
        {
            return NotFound(ApiResponse<ZgloszenieDto>.ErrorResponse("Zgłoszenie nie znalezione"));
        }

        return Ok(ApiResponse<ZgloszenieDto>.SuccessResponse(zgloszenie, "Zgłoszenie zaktualizowane"));
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<ZgloszenieDto>>> UpdateStatus(
        int id,
        [FromBody] UpdateStatusRequest request)
    {
        var userId = GetCurrentUserId();
        var zgloszenie = await _zgloszeniaService.UpdateStatusAsync(id, request, userId);

        if (zgloszenie == null)
        {
            return NotFound(ApiResponse<ZgloszenieDto>.ErrorResponse("Zgłoszenie nie znalezione"));
        }

        return Ok(ApiResponse<ZgloszenieDto>.SuccessResponse(zgloszenie, "Status zaktualizowany"));
    }

    [HttpPost("{id}/notatka")]
    public async Task<ActionResult<ApiResponse<DzialanieDto>>> AddNotatka(
        int id,
        [FromBody] CreateNotatkaRequest request)
    {
        var userId = GetCurrentUserId();

        // Check if zgłoszenie exists
        var zgloszenieExists = await _context.Zgloszenia.AnyAsync(z => z.Id == id);
        if (!zgloszenieExists)
        {
            return NotFound(ApiResponse<DzialanieDto>.ErrorResponse("Zgłoszenie nie znalezione"));
        }

        var dzialanie = new Dzialanie
        {
            IdZgloszenia = id,
            IdUzytkownika = userId,
            TypDzialania = "notatka",
            Opis = request.Opis,
            DataDzialania = DateTime.UtcNow
        };

        _context.Dzialania.Add(dzialanie);

        // Update zgłoszenie modification date
        var zgloszenie = await _context.Zgloszenia.FindAsync(id);
        if (zgloszenie != null)
        {
            zgloszenie.DataModyfikacji = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        // Load user for response
        await _context.Entry(dzialanie).Reference(d => d.User).LoadAsync();

        var dzialanieDto = new DzialanieDto
        {
            Id = dzialanie.Id,
            TypDzialania = dzialanie.TypDzialania,
            Opis = dzialanie.Opis,
            DataDzialania = dzialanie.DataDzialania,
            User = new UserDto
            {
                Id = dzialanie.User.Id,
                Login = dzialanie.User.Login,
                NazwaWyswietlana = dzialanie.User.DisplayName,
                Email = dzialanie.User.Email,
                Aktywny = dzialanie.User.IsActive
            }
        };

        return CreatedAtAction(
            nameof(GetZgloszenie),
            new { id = id },
            ApiResponse<DzialanieDto>.SuccessResponse(dzialanieDto, "Notatka dodana"));
    }

    [HttpGet("{id}/dzialania")]
    public async Task<ActionResult<ApiResponse<List<DzialanieDto>>>> GetDzialania(int id)
    {
        var dzialania = await _context.Dzialania
            .Where(d => d.IdZgloszenia == id)
            .Include(d => d.User)
            .OrderByDescending(d => d.DataDzialania)
            .Select(d => new DzialanieDto
            {
                Id = d.Id,
                TypDzialania = d.TypDzialania,
                Opis = d.Opis,
                DataDzialania = d.DataDzialania,
                StatusPoprzedni = d.StatusPoprzedni,
                StatusNowy = d.StatusNowy,
                User = new UserDto
                {
                    Id = d.User.Id,
                    Login = d.User.Login,
                    NazwaWyswietlana = d.User.DisplayName,
                    Email = d.User.Email,
                    Aktywny = d.User.IsActive
                }
            })
            .ToListAsync();

        return Ok(ApiResponse<List<DzialanieDto>>.SuccessResponse(dzialania));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteZgloszenie(int id)
    {
        var success = await _zgloszeniaService.DeleteZgloszenieAsync(id);

        if (!success)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Zgłoszenie nie znalezione"));
        }

        return Ok(ApiResponse<object>.SuccessResponse(null, "Zgłoszenie usunięte"));
    }
}
