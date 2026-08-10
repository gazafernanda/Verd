using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Verd.Api.Data;
using Verd.Api.DTOs.Admin;
using Verd.Api.Models;
using Verd.Api.Services;

namespace Verd.Api.Controllers;

/// <summary>
/// Admin console backing UC15 (Manage User Account) and UC16 (System Setting
/// Management). Every route requires the Admin role, which is carried in the JWT.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController(AppDbContext db) : ControllerBase
{
    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")!);

    // ── Dashboard ────────────────────────────────────────────────────────────
    [HttpGet("stats")]
    public async Task<ActionResult<AdminStatsDto>> GetStats()
    {
        var weekAgo = DateTime.UtcNow.AddDays(-7);
        var now = DateTime.UtcNow;

        var users = await db.Users.ToListAsync();
        var plants = await db.Plants.ToListAsync();

        // Status is derived, not stored, so count it the same way the app does.
        var needingCare = plants.Count(p =>
            PlantCareService.StatusFor(PlantCareService.CurrentLevel(p, now)) != "HEALTHY");

        return Ok(new AdminStatsDto(
            TotalUsers: users.Count,
            TotalAdmins: users.Count(u => u.Role == "Admin"),
            TotalPlants: plants.Count,
            TotalLogs: await db.PlantLogs.CountAsync(),
            PlantsNeedingCare: needingCare,
            NewUsersThisWeek: users.Count(u => u.MemberSince >= weekAgo)
        ));
    }

    // ── UC15: Manage User Account ────────────────────────────────────────────
    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<AdminUserDto>>> GetUsers([FromQuery] string? search = null)
    {
        var query = db.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(u =>
                u.DisplayName.ToLower().Contains(term) || u.Email.ToLower().Contains(term));
        }

        var users = await query
            .OrderByDescending(u => u.MemberSince)
            .Select(u => new AdminUserDto(
                u.Id, u.DisplayName, u.Email, u.Location, u.Tier, u.Role,
                u.MemberSince, u.WeatherAlertsEnabled,
                u.Plants.Count,
                u.Plants.SelectMany(p => p.Logs).Count()))
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("users/{id}")]
    public async Task<ActionResult<AdminUserDto>> GetUser(int id)
    {
        var user = await db.Users
            .Include(u => u.Plants).ThenInclude(p => p.Logs)
            .FirstOrDefaultAsync(u => u.Id == id);

        return user is null ? NotFound() : Ok(ToDto(user));
    }

    [HttpPut("users/{id}")]
    public async Task<ActionResult<AdminUserDto>> UpdateUser(int id, UpdateAdminUserDto dto)
    {
        var user = await db.Users
            .Include(u => u.Plants).ThenInclude(p => p.Logs)
            .FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) return NotFound();

        if (dto.Role is not ("Admin" or "Gardener"))
            return BadRequest(new { message = "Role must be Admin or Gardener." });

        // Losing the last admin would lock everyone out of this console.
        if (user.Role == "Admin" && dto.Role != "Admin" &&
            await db.Users.CountAsync(u => u.Role == "Admin") <= 1)
            return BadRequest(new { message = "Cannot demote the last remaining admin." });

        if (!string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase) &&
            await db.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id))
            return Conflict(new { message = "Email already in use." });

        user.DisplayName = dto.DisplayName;
        user.Email = dto.Email;
        user.Location = dto.Location;
        user.Tier = dto.Tier;
        user.Role = dto.Role;
        user.WeatherAlertsEnabled = dto.WeatherAlertsEnabled;

        await db.SaveChangesAsync();
        return Ok(ToDto(user));
    }

    [HttpPost("users/{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(int id, ResetPasswordDto dto)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return NotFound();

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return NotFound();

        if (id == CurrentUserId)
            return BadRequest(new { message = "You cannot delete your own account." });

        if (user.Role == "Admin" && await db.Users.CountAsync(u => u.Role == "Admin") <= 1)
            return BadRequest(new { message = "Cannot delete the last remaining admin." });

        // Plants and logs cascade, so removing the user cleans up their garden too.
        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return NoContent();
    }

    // ── UC16: System Setting Management ──────────────────────────────────────
    [HttpGet("settings")]
    public async Task<ActionResult<IEnumerable<SystemSettingDto>>> GetSettings()
    {
        var stored = await db.SystemSettings.ToDictionaryAsync(s => s.Key, s => s);

        // Surface every known setting, falling back to the shipped default so the
        // console shows the full list even before anything has been overridden.
        var result = SystemSetting.Defaults.Select(d => stored.TryGetValue(d.Key, out var s)
            ? new SystemSettingDto(s.Key, s.Value, s.UpdatedAt)
            : new SystemSettingDto(d.Key, d.Value, DateTime.MinValue));

        return Ok(result);
    }

    [HttpPut("settings")]
    public async Task<ActionResult<IEnumerable<SystemSettingDto>>> UpdateSettings(UpdateSystemSettingsDto dto)
    {
        foreach (var (key, value) in dto.Settings)
        {
            if (!SystemSetting.Defaults.ContainsKey(key))
                return BadRequest(new { message = $"Unknown setting: {key}" });

            var existing = await db.SystemSettings.FirstOrDefaultAsync(s => s.Key == key);
            if (existing is null)
            {
                db.SystemSettings.Add(new SystemSetting
                {
                    Key = key,
                    Value = value,
                    UpdatedAt = DateTime.UtcNow,
                });
            }
            else
            {
                existing.Value = value;
                existing.UpdatedAt = DateTime.UtcNow;
            }
        }

        await db.SaveChangesAsync();
        return await GetSettings();
    }

    private static AdminUserDto ToDto(User u) => new(
        u.Id, u.DisplayName, u.Email, u.Location, u.Tier, u.Role,
        u.MemberSince, u.WeatherAlertsEnabled,
        u.Plants.Count,
        u.Plants.SelectMany(p => p.Logs).Count());
}
