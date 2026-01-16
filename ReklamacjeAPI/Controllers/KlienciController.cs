using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReklamacjeAPI.Data;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Models;

namespace ReklamacjeAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class KlienciController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public KlienciController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<KlientDto>>>> GetKlienci(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest(ApiResponse<PaginatedResponse<KlientDto>>.ErrorResponse(
                "Nieprawidłowe parametry paginacji"));
        }

        var totalItems = await _context.Klienci.CountAsync();

        var klienci = await _context.Klienci
            .OrderBy(k => k.ImieNazwisko)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(k => new KlientDto
            {
                Id = k.Id,
                ImieNazwisko = k.ImieNazwisko,
                Telefon = k.Telefon,
                Email = k.Email,
                Adres = k.Adres,
                Miasto = k.Miasto
            })
            .ToListAsync();

        var response = new PaginatedResponse<KlientDto>
        {
            Items = klienci,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };

        return Ok(ApiResponse<PaginatedResponse<KlientDto>>.SuccessResponse(response));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<KlientDto>>> GetKlient(int id)
    {
        var klient = await _context.Klienci.FindAsync(id);

        if (klient == null)
        {
            return NotFound(ApiResponse<KlientDto>.ErrorResponse("Klient nie znaleziony"));
        }

        var klientDto = new KlientDto
        {
            Id = klient.Id,
            ImieNazwisko = klient.ImieNazwisko,
            Telefon = klient.Telefon,
            Email = klient.Email,
            Adres = klient.Adres,
            Miasto = klient.Miasto
        };

        return Ok(ApiResponse<KlientDto>.SuccessResponse(klientDto));
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<List<KlientDto>>>> SearchKlienci([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            return BadRequest(ApiResponse<List<KlientDto>>.ErrorResponse(
                "Zapytanie musi zawierać co najmniej 2 znaki"));
        }

        var klienci = await _context.Klienci
            .Where(k => k.ImieNazwisko.Contains(query) || 
                       (k.Telefon != null && k.Telefon.Contains(query)) ||
                       (k.Email != null && k.Email.Contains(query)))
            .Take(20)
            .Select(k => new KlientDto
            {
                Id = k.Id,
                ImieNazwisko = k.ImieNazwisko,
                Telefon = k.Telefon,
                Email = k.Email,
                Adres = k.Adres,
                Miasto = k.Miasto
            })
            .ToListAsync();

        return Ok(ApiResponse<List<KlientDto>>.SuccessResponse(klienci));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<KlientDto>>> CreateKlient([FromBody] CreateKlientRequest request)
    {
        var klient = new Klient
        {
            ImieNazwisko = request.ImieNazwisko,
            Telefon = request.Telefon,
            Email = request.Email,
            Adres = request.Adres,
            KodPocztowy = request.KodPocztowy,
            Miasto = request.Miasto,
            Uwagi = request.Uwagi,
            DataDodania = DateTime.UtcNow
        };

        _context.Klienci.Add(klient);
        await _context.SaveChangesAsync();

        var klientDto = new KlientDto
        {
            Id = klient.Id,
            ImieNazwisko = klient.ImieNazwisko,
            Telefon = klient.Telefon,
            Email = klient.Email,
            Adres = klient.Adres,
            Miasto = klient.Miasto
        };

        return CreatedAtAction(
            nameof(GetKlient),
            new { id = klient.Id },
            ApiResponse<KlientDto>.SuccessResponse(klientDto, "Klient utworzony pomyślnie"));
    }
}
