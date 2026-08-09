using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Verd.Api.Data;
using Verd.Api.DTOs.Plants;
using Verd.Api.Models;
using Verd.Api.Services;

namespace Verd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlantsController(AppDbContext db, IHttpClientFactory httpFactory) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")!);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlantDto>>> GetAll()
    {
        var plants = await db.Plants
            .Where(p => p.UserId == UserId)
            .ToListAsync();

        return Ok(plants.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlantDto>> GetById(int id)
    {
        var plant = await db.Plants.FirstOrDefaultAsync(p => p.Id == id && p.UserId == UserId);
        return plant is null ? NotFound() : Ok(ToDto(plant));
    }

    [HttpPost("validate")]
    public async Task<ActionResult<PlantValidationResult>> Validate([FromBody] ValidatePlantNameDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return Ok(new PlantValidationResult(false));

        var client = httpFactory.CreateClient("Groq");
        var body = new
        {
            model = "llama-3.3-70b-versatile",
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = $"Is \"{dto.Name.Trim()}\" a real plant, plant species, or plant variety? Reply with only \"yes\" or \"no\"."
                }
            },
            temperature = 0,
            max_tokens = 5
        };

        try
        {
            var response = await client.PostAsJsonAsync("chat/completions", body);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<GroqValidationResponse>();
            var answer = result?.Choices?[0].Message.Content.Trim().ToLower() ?? "";
            return Ok(new PlantValidationResult(answer.StartsWith("yes")));
        }
        catch
        {
            // If Groq is unavailable, allow the plant through
            return Ok(new PlantValidationResult(true));
        }
    }

    [HttpPost]
    public async Task<ActionResult<PlantDto>> Create(UpsertPlantDto dto)
    {
        var plant = new Plant
        {
            UserId = UserId,
            Name = dto.Name,
            Status = PlantCareService.StatusFor(dto.WateringLevel),
            WateringLevel = dto.WateringLevel,
            LastWatered = dto.LastWatered,
            // Anchor the chosen level in time so it starts decaying from here.
            LastWateredAt = PlantCareService.BaselineFor(
                dto.WateringLevel, dto.WateringFrequency, DateTime.UtcNow),
            Category = dto.Category,
            IconBg = dto.IconBg,
            WateringFrequency = dto.WateringFrequency,
            Sunlight = dto.Sunlight,
            Notes = dto.Notes,
            CareCategory = dto.CareCategory,
            CareTitle = dto.CareTitle,
            CareDescription = dto.CareDescription,
            CareImage = dto.CareImage,
            CareBgType = dto.CareBgType,
        };

        db.Plants.Add(plant);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = plant.Id }, ToDto(plant));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PlantDto>> Update(int id, UpsertPlantDto dto)
    {
        var plant = await db.Plants.FirstOrDefaultAsync(p => p.Id == id && p.UserId == UserId);
        if (plant is null) return NotFound();

        // Re-anchor only when the user actually moved the slider (or the cycle
        // changed) — otherwise an unrelated edit would silently top the plant up.
        var currentLevel = PlantCareService.CurrentLevel(plant, DateTime.UtcNow);
        if (dto.WateringLevel != currentLevel || dto.WateringFrequency != plant.WateringFrequency)
        {
            plant.LastWateredAt = PlantCareService.BaselineFor(
                dto.WateringLevel, dto.WateringFrequency, DateTime.UtcNow);
        }

        plant.Name = dto.Name;
        plant.WateringLevel = dto.WateringLevel;
        plant.Status = PlantCareService.StatusFor(dto.WateringLevel);
        plant.LastWatered = dto.LastWatered;
        plant.Category = dto.Category;
        plant.IconBg = dto.IconBg;
        plant.WateringFrequency = dto.WateringFrequency;
        plant.Sunlight = dto.Sunlight;
        plant.Notes = dto.Notes;
        plant.CareCategory = dto.CareCategory;
        plant.CareTitle = dto.CareTitle;
        plant.CareDescription = dto.CareDescription;
        plant.CareImage = dto.CareImage;
        plant.CareBgType = dto.CareBgType;

        await db.SaveChangesAsync();
        return Ok(ToDto(plant));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var plant = await db.Plants.FirstOrDefaultAsync(p => p.Id == id && p.UserId == UserId);
        if (plant is null) return NotFound();

        db.Plants.Remove(plant);
        await db.SaveChangesAsync();
        return NoContent();
    }

    // Water level is derived at read time rather than stored, so a plant dries out
    // on its own between requests without needing a background job.
    private static PlantDto ToDto(Plant p)
    {
        var now = DateTime.UtcNow;
        var level = PlantCareService.CurrentLevel(p, now);
        return ToDto(p, level, PlantCareService.StatusFor(level), PlantCareService.LastWateredLabel(p, now));
    }

    private static PlantDto ToDto(Plant p, int wateringLevel, string status, string lastWatered) => new(
        Id: p.Id,
        Name: p.Name,
        Status: status,
        WateringLevel: wateringLevel,
        LastWatered: lastWatered,
        Category: p.Category,
        IconBg: p.IconBg,
        WateringFrequency: p.WateringFrequency,
        Sunlight: p.Sunlight,
        Notes: p.Notes,
        CareCard: new CareCardDto(
            Category: p.CareCategory,
            Title: p.CareTitle,
            Description: p.CareDescription,
            Image: p.CareImage,
            BgType: p.CareBgType
        )
    );
}

public record ValidatePlantNameDto(string Name);
public record PlantValidationResult(bool IsValid);

file record GroqValidationResponse(
    [property: JsonPropertyName("choices")] List<GroqValidationChoice>? Choices
);
file record GroqValidationChoice(
    [property: JsonPropertyName("message")] GroqValidationMessage Message
);
file record GroqValidationMessage(
    [property: JsonPropertyName("content")] string Content
);
