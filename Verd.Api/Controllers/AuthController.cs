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
        Tier: user.Tier,
        Role: user.Role
    );
}
