using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Verd.Api.Data;
using Verd.Api.DTOs.Auth;
using Verd.Api.Models;
using Verd.Api.Services;

namespace Verd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext db, JwtService jwt) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        if (await db.Users.AnyAsync(u => u.Email == dto.Email))
            return Conflict(new { message = "Email already in use." });

        var user = new User
        {
            DisplayName = dto.DisplayName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Location = dto.Location,
            MemberSince = DateTime.UtcNow,
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        // Seed default plants for new user
        db.Plants.AddRange(DefaultPlants(user.Id));
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Register), ToResponse(user));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password." });

        return Ok(ToResponse(user));
    }

    private AuthResponseDto ToResponse(User user) => new(
        Token: jwt.GenerateToken(user),
        DisplayName: user.DisplayName,
        Email: user.Email,
        Location: user.Location,
        Tier: user.Tier
    );

    private static IEnumerable<Plant> DefaultPlants(int userId) =>
    [
        new Plant { UserId = userId, Name = "Monstera Deliciosa", Status = "HEALTHY", WateringLevel = 65, LastWatered = "3 days ago", Category = "Tropical Plants", IconBg = "#f0f9f4", CareCategory = "WATERING", CareTitle = "Check the Monstera", CareDescription = "Soil is slightly dry. The current humidity may not be sufficient.", CareImage = "/src/assets/monstera.png", CareBgType = "blue" },
        new Plant { UserId = userId, Name = "Snake Plant", Status = "NEEDS WATER", WateringLevel = 15, LastWatered = "14 days ago", Category = "Succulents & Cacti", IconBg = "#111111", CareCategory = "POSITIONING", CareTitle = "Rotate Succulents", CareDescription = "Ensure even growth by rotating toward the light source.", CareImage = "/src/assets/succulents.png", CareBgType = "yellow" },
        new Plant { UserId = userId, Name = "Fiddle Leaf Fig", Status = "HEALTHY", WateringLevel = 75, LastWatered = "2 days ago", Category = "Tropical Plants", IconBg = "#f0f9f4", CareCategory = "MAINTENANCE", CareTitle = "Seasonal Pruning", CareDescription = "A good time to remove dead stems and shape the plant.", CareImage = "/src/assets/pruning.png", CareBgType = "green" },
        new Plant { UserId = userId, Name = "Boston Fern", Status = "NEEDS MISTING", WateringLevel = 50, LastWatered = "1 day ago", Category = "Ferns", IconBg = "#e6f3ef", CareCategory = "FERTILIZING", CareTitle = "Feed the Boston Fern", CareDescription = "Spring is here, time to add liquid fertilizer.", CareImage = "https://images.unsplash.com/photo-1614594975525-e45190c55d40?w=600&h=400&fit=crop", CareBgType = "green" },
    ];
}
