using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Verd.Api.Data;
using Verd.Api.DTOs.Plants;
using Verd.Api.Models;
using Verd.Api.Services;

namespace Verd.Api.Controllers;

[ApiController]
[Route("api/plants/{plantId}/logs")]
[Authorize]
public class PlantLogsController(AppDbContext db) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")!);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlantLogDto>>> GetLogs(int plantId)
    {
        var plant = await db.Plants.FirstOrDefaultAsync(p => p.Id == plantId && p.UserId == UserId);
        if (plant is null) return NotFound();

        var logs = await db.PlantLogs
            .Where(l => l.PlantId == plantId)
            .OrderByDescending(l => l.LoggedAt)
            .Select(l => new PlantLogDto(l.Id, l.PlantId, l.Action, l.Notes, l.LoggedAt))
            .ToListAsync();

        return Ok(logs);
    }

    [HttpPost]
    public async Task<ActionResult<PlantLogDto>> CreateLog(int plantId, CreatePlantLogDto dto)
    {
        var plant = await db.Plants.FirstOrDefaultAsync(p => p.Id == plantId && p.UserId == UserId);
        if (plant is null) return NotFound();

        var log = new PlantLog
        {
            PlantId = plantId,
            Action = dto.Action,
            Notes = dto.Notes,
            LoggedAt = DateTime.UtcNow,
        };

        // Update plant state based on action
        if (dto.Action == "watered")
        {
            plant.WateringLevel = 100;
            plant.LastWatered = "Just now";
            plant.LastWateredAt = log.LoggedAt;
            plant.Status = "HEALTHY";
        }
        else if (dto.Action == "misted")
        {
            // Misting is a partial top-up: it clears NEEDS MISTING but does not
            // reset the watering cycle the way a full watering does.
            var level = PlantCareService.CurrentLevel(plant, log.LoggedAt);
            if (level <= PlantCareService.NeedsMistingAtOrBelow)
            {
                var topUp = Math.Max(level, PlantCareService.NeedsMistingAtOrBelow + 1);
                plant.WateringLevel = topUp;
                plant.LastWateredAt = PlantCareService.BaselineFor(
                    topUp, plant.WateringFrequency, log.LoggedAt);
                plant.Status = PlantCareService.StatusFor(topUp);
            }
        }

        db.PlantLogs.Add(log);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLogs), new { plantId },
            new PlantLogDto(log.Id, log.PlantId, log.Action, log.Notes, log.LoggedAt));
    }

    [HttpDelete("{logId}")]
    public async Task<IActionResult> DeleteLog(int plantId, int logId)
    {
        var plant = await db.Plants.FirstOrDefaultAsync(p => p.Id == plantId && p.UserId == UserId);
        if (plant is null) return NotFound();

        var log = await db.PlantLogs.FirstOrDefaultAsync(l => l.Id == logId && l.PlantId == plantId);
        if (log is null) return NotFound();

        db.PlantLogs.Remove(log);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
