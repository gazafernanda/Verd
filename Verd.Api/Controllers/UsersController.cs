using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Verd.Api.Data;
using Verd.Api.DTOs.Users;

namespace Verd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(AppDbContext db) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")!);

    [HttpGet("profile")]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        var user = await db.Users.FindAsync(UserId);
        if (user is null) return NotFound();

        return Ok(new UserProfileDto(
            Id: user.Id,
            DisplayName: user.DisplayName,
            Email: user.Email,
            Location: user.Location,
            AvatarUrl: user.AvatarUrl,
            Tier: user.Tier,
            MemberSince: user.MemberSince.ToString("MMM yyyy"),
            WeatherAlertsEnabled: user.WeatherAlertsEnabled
        ));
    }

    [HttpPatch("settings")]
    public async Task<ActionResult<UserProfileDto>> UpdateSettings(UpdateSettingsDto dto)
    {
        var user = await db.Users.FindAsync(UserId);
        if (user is null) return NotFound();

        if (dto.AvatarUrl is { Length: > 2_000_000 })
            return BadRequest(new { message = "Avatar image is too large." });

        if (dto.DisplayName is not null) user.DisplayName = dto.DisplayName;
        if (dto.Location is not null) user.Location = dto.Location;
        if (dto.AvatarUrl is not null) user.AvatarUrl = dto.AvatarUrl;
        if (dto.WeatherAlertsEnabled is not null) user.WeatherAlertsEnabled = dto.WeatherAlertsEnabled.Value;

        await db.SaveChangesAsync();

        return Ok(new UserProfileDto(
            Id: user.Id,
            DisplayName: user.DisplayName,
            Email: user.Email,
            Location: user.Location,
            AvatarUrl: user.AvatarUrl,
            Tier: user.Tier,
            MemberSince: user.MemberSince.ToString("MMM yyyy"),
            WeatherAlertsEnabled: user.WeatherAlertsEnabled
        ));
    }
}
