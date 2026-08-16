using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Verd.Api.Data;
using Verd.Api.DTOs.Recommendations;
using Verd.Api.Services;

namespace Verd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[RequireVerifiedEmail]
public class RecommendationsController(AppDbContext db, RecommendationAiService aiService) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")!);

    [HttpPost("generate")]
    public async Task<ActionResult<RecommendationDto>> Generate([FromBody] GeneratePlantRecommendationDto dto)
    {
        var plant = await db.Plants.FindAsync(dto.PlantId);
        if (plant is null || plant.UserId != UserId || plant.DeletedAt is not null) return NotFound();

        var user = await db.Users.FindAsync(UserId);
        if (user is null) return NotFound();

        var result = await aiService.GenerateAsync(plant, user.Location, dto.Language);
        if (result is null) return StatusCode(502, "AI recommendation service unavailable.");

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<RecommendationDto>> Get([FromQuery] string? lang = null)
    {
        var plants = await db.Plants
            .Where(p => p.UserId == UserId)
            .ToListAsync();

        if (plants.Count == 0)
            return Ok(new RecommendationDto([], null));

        var user = await db.Users.FindAsync(UserId);
        if (user is null) return NotFound();

        var result = await aiService.GenerateForGardenAsync(plants, user.Location, lang);
        if (result is null) return StatusCode(502, "AI recommendation service unavailable.");

        return Ok(result);
    }
}
