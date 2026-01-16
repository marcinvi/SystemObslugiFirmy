using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Services;

namespace ReklamacjeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    [HttpPost("upload")]
    public async Task<ActionResult<ApiResponse<FileUploadResponse>>> Upload(
        IFormFile file,
        [FromForm] int? zgloszenieId = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _fileService.UploadFileAsync(file, zgloszenieId, userId);

            return Ok(new ApiResponse<FileUploadResponse>
            {
                Success = true,
                Data = result,
                Message = "Plik przesłany"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<FileUploadResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Download(int id)
    {
        var plik = await _fileService.GetFileByIdAsync(id);

        if (plik == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Plik nie znaleziony"
            });
        }

        if (!System.IO.File.Exists(plik.SciezkaPliku))
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Plik nie istnieje na dysku"
            });
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(plik.SciezkaPliku);
        return File(fileBytes, plik.TypPliku ?? "application/octet-stream", plik.NazwaOryginalnaPliku);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var userId = GetCurrentUserId();
        var result = await _fileService.DeleteFileAsync(id, userId);

        if (!result)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Plik nie znaleziony"
            });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Plik usunięty"
        });
    }
}
