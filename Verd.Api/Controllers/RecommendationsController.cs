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
public class RecommendationsController(AppDbContext db, RecommendationAiService aiService) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")!);

    [HttpPost("generate")]
    public async Task<ActionResult<RecommendationDto>> Generate([FromBody] GeneratePlantRecommendationDto dto)
    {
        var plant = await db.Plants.FindAsync(dto.PlantId);
        if (plant is null || plant.UserId != UserId) return NotFound();

        var user = await db.Users.FindAsync(UserId);
        if (user is null) return NotFound();

        var result = await aiService.GenerateAsync(plant, user.Location);
        if (result is null) return StatusCode(502, "AI recommendation service unavailable.");

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<RecommendationDto>> Get()
    {
        var plants = await db.Plants
            .Where(p => p.UserId == UserId)
            .ToListAsync();

        var actions = new List<PriorityActionDto>();

        // Plants needing water → immediate action
        var needWater = plants.Where(p => p.Status == "NEEDS WATER").ToList();
        if (needWater.Count > 0)
        {
            var names = string.Join(", ", needWater.Select(p => p.Name));
            actions.Add(new PriorityActionDto(
                Id: "water-plants",
                Title: "Water your plants",
                Description: $"{names} {(needWater.Count == 1 ? "needs" : "need")} watering now. Apply water at the base to avoid leaf burn.",
                Priority: "IMMEDIATE",
                Type: "water"
            ));
        }

        // Plants needing misting
        var needMist = plants.Where(p => p.Status == "NEEDS MISTING").ToList();
        if (needMist.Count > 0)
        {
            var names = string.Join(", ", needMist.Select(p => p.Name));
            actions.Add(new PriorityActionDto(
                Id: "mist-plants",
                Title: "Mist for humidity",
                Description: $"{names} would benefit from misting to raise local humidity around the foliage.",
                Priority: "RECOMMENDED",
                Type: "mist"
            ));
        }

        // General recommendations when garden is healthy
        if (actions.Count == 0)
        {
            actions.Add(new PriorityActionDto(
                Id: "check-soil",
                Title: "Check soil moisture",
                Description: "All your plants look healthy. Do a finger test 2 cm deep — water if the soil feels dry.",
                Priority: "OPTIONAL",
                Type: "water"
            ));
        }

        // If garden has outdoor / vegetable plants, recommend shade cloth on high UV days
        bool hasOutdoor = plants.Any(p =>
            p.Category.Contains("Vegetable", StringComparison.OrdinalIgnoreCase) ||
            p.Category.Contains("Outdoor", StringComparison.OrdinalIgnoreCase) ||
            p.Sunlight == "full-sun" || p.Sunlight == "partial");

        if (hasOutdoor)
        {
            actions.Add(new PriorityActionDto(
                Id: "shade-cloth",
                Title: "Deploy 30% Shade Cloth",
                Description: "Protect leafy outdoor plants from intense afternoon sun to prevent tip burn and bolting.",
                Priority: "RECOMMENDED",
                Type: "shade"
            ));
        }

        // Build botanical insight based on garden composition
        BotanicalInsightDto insight;
        if (needWater.Count > 0)
        {
            insight = new BotanicalInsightDto(
                Headline: "Water stress triggers stomatal closure, halting photosynthesis.",
                Detail: "When soil moisture drops below the wilting point, plants close their stomata to conserve water — growth stops until hydration is restored."
            );
        }
        else if (plants.Any(p => p.Sunlight == "full-sun" || p.Sunlight == "partial"))
        {
            insight = new BotanicalInsightDto(
                Headline: "High solar radiation increases the transpiration rate of your crops.",
                Detail: "Stomata are wide open to facilitate cooling, leading to rapid water loss. Monitor soil moisture closely on sunny days."
            );
        }
        else
        {
            insight = new BotanicalInsightDto(
                Headline: "Consistent indirect light supports steady, compact growth.",
                Detail: "Your indoor plants are getting stable light conditions. Rotate them 90° every two weeks to keep growth even on all sides."
            );
        }

        return Ok(new RecommendationDto(actions, insight));
    }
}
