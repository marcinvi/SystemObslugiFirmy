using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Services;

namespace ReklamacjeAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/messages")]
public class MessagesController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public MessagesController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<MessageDto>>>> GetMessages([FromQuery] int? returnId = null)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var query = @"
            SELECT Id, NadawcaId, OdbiorcaId, Tytul, Tresc, DataWyslania, DotyczyZwrotuId, IFNULL(CzyPrzeczytana, 0) AS CzyPrzeczytana
            FROM Wiadomosci";

        if (returnId.HasValue)
        {
            query += " WHERE DotyczyZwrotuId = @id";
        }
        query += " ORDER BY DataWyslania DESC";

        var command = new MySqlCommand(query, connection);
        if (returnId.HasValue)
        {
            command.Parameters.AddWithValue("@id", returnId.Value);
        }

        var messages = new List<MessageDto>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            messages.Add(new MessageDto
            {
                Id = reader.GetInt32("Id"),
                NadawcaId = reader.GetInt32("NadawcaId"),
                OdbiorcaId = reader.GetInt32("OdbiorcaId"),
                Tytul = reader.IsDBNull("Tytul") ? null : reader.GetString("Tytul"),
                Tresc = reader.GetString("Tresc"),
                DataWyslania = reader.GetDateTime("DataWyslania"),
                DotyczyZwrotuId = reader.IsDBNull("DotyczyZwrotuId") ? null : reader.GetInt32("DotyczyZwrotuId"),
                CzyPrzeczytana = reader.GetInt32("CzyPrzeczytana") == 1
            });
        }

        return Ok(ApiResponse<List<MessageDto>>.SuccessResponse(messages));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MessageDto>>> CreateMessage([FromBody] MessageCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Tresc))
        {
            return BadRequest(ApiResponse<MessageDto>.ErrorResponse("Treść wiadomości nie może być pusta"));
        }

        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var command = new MySqlCommand(@"
            INSERT INTO Wiadomosci (NadawcaId, OdbiorcaId, Tytul, Tresc, DataWyslania, DotyczyZwrotuId, CzyPrzeczytana)
            VALUES (@nadawca, @odbiorca, @tytul, @tresc, @data, @zwrotId, 0);
            SELECT LAST_INSERT_ID();", connection);
        command.Parameters.AddWithValue("@nadawca", request.NadawcaId);
        command.Parameters.AddWithValue("@odbiorca", request.OdbiorcaId);
        command.Parameters.AddWithValue("@tytul", request.Tytul ?? string.Empty);
        command.Parameters.AddWithValue("@tresc", request.Tresc.Trim());
        command.Parameters.AddWithValue("@data", DateTime.Now);
        command.Parameters.AddWithValue("@zwrotId", request.DotyczyZwrotuId ?? (object)DBNull.Value);

        var newId = Convert.ToInt32(await command.ExecuteScalarAsync());

        var dto = new MessageDto
        {
            Id = newId,
            NadawcaId = request.NadawcaId,
            OdbiorcaId = request.OdbiorcaId,
            Tytul = request.Tytul,
            Tresc = request.Tresc.Trim(),
            DataWyslania = DateTime.Now,
            DotyczyZwrotuId = request.DotyczyZwrotuId,
            CzyPrzeczytana = false
        };

        return CreatedAtAction(nameof(GetMessages), new { returnId = request.DotyczyZwrotuId },
            ApiResponse<MessageDto>.SuccessResponse(dto));
    }

    [HttpPatch("{id:int}/read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkRead(int id)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var command = new MySqlCommand(@"
            UPDATE Wiadomosci
            SET CzyPrzeczytana = 1
            WHERE Id = @id;", connection);
        command.Parameters.AddWithValue("@id", id);

        var updated = await command.ExecuteNonQueryAsync();
        if (updated == 0)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Wiadomość nie znaleziona"));
        }

        return Ok(ApiResponse<object>.SuccessResponse(null, "Wiadomość oznaczona jako przeczytana"));
    }
}
