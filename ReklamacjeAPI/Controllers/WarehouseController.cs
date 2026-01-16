using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Services;

namespace ReklamacjeAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/warehouse")]
public class WarehouseController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public WarehouseController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<List<WarehouseSearchItemDto>>>> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest(ApiResponse<List<WarehouseSearchItemDto>>.ErrorResponse("Podaj frazę wyszukiwania"));
        }

        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        const string sql = @"
            SELECT
                z.NrZgloszenia,
                IFNULL(z.NrSeryjny, '') AS SN,
                IFNULL(p.NazwaSystemowa, 'Brak modelu') AS Model,
                IFNULL(k.ImieNazwisko, 'Brak klienta') AS Klient,
                p.Id AS ProduktId,
                p.Kategoria
            FROM Zgloszenia z
            LEFT JOIN Produkty p ON z.ProduktID = p.Id
            LEFT JOIN Klienci k ON z.KlientID = k.Id
            WHERE
                z.NrZgloszenia LIKE @q OR
                z.NrSeryjny LIKE @q OR
                p.NazwaSystemowa LIKE @q OR
                k.ImieNazwisko LIKE @q;";

        var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@q", $"%{query.Trim()}%");

        var results = new List<WarehouseSearchItemDto>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new WarehouseSearchItemDto
            {
                NrZgloszenia = reader.GetString("NrZgloszenia"),
                Sn = reader.IsDBNull("SN") ? null : reader.GetString("SN"),
                Model = reader.IsDBNull("Model") ? null : reader.GetString("Model"),
                Klient = reader.IsDBNull("Klient") ? null : reader.GetString("Klient"),
                ProduktId = reader.IsDBNull("ProduktId") ? 0 : reader.GetInt32("ProduktId"),
                Kategoria = reader.IsDBNull("Kategoria") ? null : reader.GetString("Kategoria")
            });
        }

        return Ok(ApiResponse<List<WarehouseSearchItemDto>>.SuccessResponse(results));
    }

    [HttpPost("intake")]
    public async Task<ActionResult<ApiResponse<object>>> Intake([FromBody] WarehouseIntakeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Model))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Model jest wymagany"));
        }

        var nrZgloszenia = string.IsNullOrWhiteSpace(request.NrZgloszenia)
            ? $"LUZ/{DateTime.Now:yyyyMMddHHmmss}"
            : request.NrZgloszenia.Trim();

        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var check = new MySqlCommand(
                "SELECT Id FROM MagazynZwrotow WHERE NrZgloszenia = @nr LIMIT 1;", connection, transaction);
            check.Parameters.AddWithValue("@nr", nrZgloszenia);
            var exists = await check.ExecuteScalarAsync();
            if (exists != null && exists != DBNull.Value)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("To zgłoszenie jest już w magazynie"));
            }

            var insert = new MySqlCommand(@"
                INSERT INTO MagazynZwrotow
                    (NrZgloszenia, Model, NumerSeryjny, DataPrzyjecia, StatusFizyczny, Lokalizacja, UwagiMagazynowe, CzyDawca)
                VALUES
                    (@nr, @model, @sn, @data, @status, @lok, @uwagi, @czyDawca);
                SELECT LAST_INSERT_ID();", connection, transaction);
            insert.Parameters.AddWithValue("@nr", nrZgloszenia);
            insert.Parameters.AddWithValue("@model", request.Model.Trim());
            insert.Parameters.AddWithValue("@sn", request.NumerSeryjny ?? string.Empty);
            insert.Parameters.AddWithValue("@data", DateTime.Now);
            insert.Parameters.AddWithValue("@status", request.Status);
            insert.Parameters.AddWithValue("@lok", request.Lokalizacja);
            insert.Parameters.AddWithValue("@uwagi", request.Uwagi ?? string.Empty);
            insert.Parameters.AddWithValue("@czyDawca", request.CzyDawca ? 1 : 0);

            var magazynId = Convert.ToInt32(await insert.ExecuteScalarAsync());

            if (request.CzyDawca && request.Czesci.Count > 0)
            {
                foreach (var part in request.Czesci)
                {
                    if (string.IsNullOrWhiteSpace(part))
                    {
                        continue;
                    }

                    var partCommand = new MySqlCommand(@"
                        INSERT INTO DostepneCzesci
                            (MagazynDawcaId, NazwaCzesci, StanOpis, CzyDostepna, TypPochodzenia)
                        VALUES
                            (@id, @nazwa, 'Używana - z demontażu', 1, 'Demontaż');", connection, transaction);
                    partCommand.Parameters.AddWithValue("@id", magazynId);
                    partCommand.Parameters.AddWithValue("@nazwa", part.Trim());
                    await partCommand.ExecuteNonQueryAsync();
                }
            }

            await transaction.CommitAsync();

            return CreatedAtAction(nameof(Search), new { query = nrZgloszenia },
                ApiResponse<object>.SuccessResponse(null, "Sprzęt zapisany w magazynie"));
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
