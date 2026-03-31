using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Verd.Api.Data;
using Verd.Api.DTOs.Plants;
using Verd.Api.Models;

namespace Verd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlantsController(AppDbContext db) : ControllerBase
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

    [HttpPost]
    public async Task<ActionResult<PlantDto>> Create(UpsertPlantDto dto)
    {
        var plant = new Plant
        {
            UserId = UserId,
            Name = dto.Name,
            Status = dto.Status,
            WateringLevel = dto.WateringLevel,
            LastWatered = dto.LastWatered,
            Category = dto.Category,
            IconBg = dto.IconBg,
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

        plant.Name = dto.Name;
        plant.Status = dto.Status;
        plant.WateringLevel = dto.WateringLevel;
        plant.LastWatered = dto.LastWatered;
        plant.Category = dto.Category;
        plant.IconBg = dto.IconBg;
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

    private static PlantDto ToDto(Plant p) => new(
        Id: p.Id,
        Name: p.Name,
        Status: p.Status,
        WateringLevel: p.WateringLevel,
        LastWatered: p.LastWatered,
        Category: p.Category,
        IconBg: p.IconBg,
        CareCard: new CareCardDto(
            Category: p.CareCategory,
            Title: p.CareTitle,
            Description: p.CareDescription,
            Image: p.CareImage,
            BgType: p.CareBgType
        )
    );
}
