using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Verd.Api.Data;
using Verd.Api.DTOs.Weather;
using Verd.Api.Services;

namespace Verd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[RequireVerifiedEmail]
public class WeatherController(AppDbContext db, WeatherService weatherService) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")!);

    [HttpGet]
    public async Task<ActionResult<WeatherDto>> GetCurrent()
    {
        var user = await db.Users.FindAsync(UserId);
        if (user is null) return NotFound();
        if (string.IsNullOrWhiteSpace(user.Location))
            return BadRequest("No location set for this user.");

        var weather = await weatherService.GetForLocationAsync(user.Location);
        if (weather is null) return StatusCode(502, "Could not fetch weather data.");

        return Ok(weather);
    }
}
