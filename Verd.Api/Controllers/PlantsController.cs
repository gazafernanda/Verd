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
[RequireVerifiedEmail]
public class PlantsController(
    AppDbContext db,
    IHttpClientFactory httpFactory,
    PlantSuggestionService suggestions) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")!);

    /// <summary>Plants currently in the garden. Soft-deleted rows live on in /history.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlantDto>>> GetAll()
    {
        var plants = await db.Plants
            .Where(p => p.UserId == UserId && p.DeletedAt == null)
            .ToListAsync();

        return Ok(plants.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PlantDto>> GetById(int id)
    {
        var plant = await db.Plants
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == UserId && p.DeletedAt == null);
        return plant is null ? NotFound() : Ok(ToDto(plant));
    }

    // ── History ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Every plant the user has ever registered, active and ended alike, newest
    /// registration first.
    /// </summary>
    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<PlantHistoryDto>>> GetHistory()
    {
        var plants = await db.Plants
            .Where(p => p.UserId == UserId)
            .OrderByDescending(p => p.RegisteredAt)
            .ThenByDescending(p => p.Id)
            .ToListAsync();

        var logCounts = await db.PlantLogs
            .Where(l => plants.Select(p => p.Id).Contains(l.PlantId))
            .GroupBy(l => l.PlantId)
            .Select(g => new { PlantId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.PlantId, x => x.Count);

        return Ok(plants.Select(p => ToHistoryDto(p, logCounts.GetValueOrDefault(p.Id))));
    }

    /// <summary>
    /// A single planting period together with the monitoring data recorded during it.
    /// Works for ended plants too — that is the whole point of the soft delete.
    /// </summary>
    [HttpGet("history/{id:int}")]
    public async Task<ActionResult<PlantHistoryDetailDto>> GetHistoryDetail(int id)
    {
        var plant = await db.Plants.FirstOrDefaultAsync(p => p.Id == id && p.UserId == UserId);
        if (plant is null) return NotFound();

        var logs = await db.PlantLogs
            .Where(l => l.PlantId == id)
            .OrderByDescending(l => l.LoggedAt)
            .Select(l => new PlantLogDto(l.Id, l.PlantId, l.Action, l.Notes, l.LoggedAt))
            .ToListAsync();

        return Ok(new PlantHistoryDetailDto(
            Summary: ToHistoryDto(plant, logs.Count),
            WateringFrequency: plant.WateringFrequency,
            Sunlight: plant.Sunlight,
            Notes: plant.Notes,
            Logs: logs
        ));
    }

    /// <summary>
    /// Works out the category and care defaults for a typed plant name, so the
    /// user doesn't have to fill the form in from scratch. Also reports whether
    /// the name is a real plant, which makes a separate /validate call redundant
    /// when the client has already asked for a suggestion.
    /// </summary>
    [HttpPost("suggest")]
    public async Task<ActionResult<PlantSuggestionDto>> Suggest([FromBody] SuggestPlantDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "A plant name is required." });

        // Guards against a stray paste turning into a huge prompt.
        if (dto.Name.Trim().Length > 100)
            return BadRequest(new { message = "That plant name is too long." });

        return Ok(await suggestions.SuggestAsync(dto.Name, dto.Language));
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
            RegisteredAt = DateTime.UtcNow,
        };

        db.Plants.Add(plant);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = plant.Id }, ToDto(plant));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PlantDto>> Update(int id, UpsertPlantDto dto)
    {
        var plant = await db.Plants
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == UserId && p.DeletedAt == null);
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

    /// <summary>
    /// Ends the planting period instead of destroying the row, so the plant and
    /// the monitoring data collected for it stay available in the history page.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var plant = await db.Plants
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == UserId && p.DeletedAt == null);
        if (plant is null) return NotFound();

        plant.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static PlantHistoryDto ToHistoryDto(Plant p, int logCount)
    {
        var now = DateTime.UtcNow;
        var isActive = p.DeletedAt is null;

        // An ended plant's care status is frozen at deletion; recomputing decay
        // for it would keep "drying out" a plant that is no longer in the garden.
        var careStatus = isActive
            ? PlantCareService.StatusFor(PlantCareService.CurrentLevel(p, now))
            : p.Status;

        var periodEnd = p.DeletedAt ?? now;
        var duration = (int)Math.Max(0, Math.Floor((periodEnd - p.RegisteredAt).TotalDays));

        return new PlantHistoryDto(
            Id: p.Id,
            Name: p.Name,
            Category: p.Category,
            IconBg: p.IconBg,
            RegisteredAt: p.RegisteredAt,
            EndedAt: p.DeletedAt,
            Status: isActive ? "ACTIVE" : "ENDED",
            CareStatus: careStatus,
            DurationDays: duration,
            LogCount: logCount
        );
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
