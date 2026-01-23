using Microsoft.AspNetCore.Mvc;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Services;
using System.Linq;

namespace ReklamacjeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModulesController : ControllerBase
{
    private readonly ModulesService _modulesService;

    public ModulesController(ModulesService modulesService)
    {
        _modulesService = modulesService;
    }

    [HttpGet("assigned")]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetAssignedModules()
    {
        var login = Request.Headers["X-User"].FirstOrDefault() ?? User.Identity?.Name ?? string.Empty;
        var modules = await _modulesService.GetAssignedModulesAsync(login);
        return Ok(ApiResponse<List<string>>.SuccessResponse(modules));
    }
}
