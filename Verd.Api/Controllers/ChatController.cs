using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Verd.Api.Data;
using Verd.Api.DTOs.Chat;
using Verd.Api.Models;
using Verd.Api.Services;

namespace Verd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[RequireVerifiedEmail]
public class ChatController(ChatService chat, AppDbContext db) : ControllerBase
{
    /// <summary>
    /// How many past turns to replay to the model. The full history is kept in the
    /// database and shown to the user; only the prompt window is trimmed.
    /// </summary>
    private const int PromptHistoryTurns = 20;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")!);

    /// <summary>
    /// The stored conversation, oldest first, so the client can render it in order
    /// and scroll back. Survives logout — only the device session is cleared.
    /// </summary>
    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<StoredChatMessageDto>>> GetHistory()
    {
        var messages = await db.ChatMessages
            .Where(m => m.UserId == UserId)
            .OrderBy(m => m.SentAt)
            .ThenBy(m => m.Id)
            .Select(m => new StoredChatMessageDto(m.Id, m.Role, m.Content, m.SentAt))
            .ToListAsync();

        return Ok(messages);
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponseDto>> Send(ChatRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Message))
            return BadRequest(new { message = "Message cannot be empty." });

        var plants = await db.Plants
            .Where(p => p.UserId == UserId && p.DeletedAt == null)
            .ToListAsync();

        // The conversation is rebuilt from the database rather than trusted from
        // the request, so history can't be forged and stays consistent across devices.
        var stored = await db.ChatMessages
            .Where(m => m.UserId == UserId)
            .OrderByDescending(m => m.SentAt)
            .ThenByDescending(m => m.Id)
            .Take(PromptHistoryTurns)
            .ToListAsync();

        var history = stored
            .OrderBy(m => m.SentAt).ThenBy(m => m.Id)
            .Select(m => new ChatMessageDto(m.Role, m.Content))
            .ToList();

        var now = DateTime.UtcNow;
        var userMessage = new ChatMessage
        {
            UserId = UserId,
            Role = "user",
            Content = dto.Message,
            SentAt = now,
        };
        db.ChatMessages.Add(userMessage);

        string reply;
        try
        {
            reply = await chat.SendAsync(dto.Message, history, plants);
        }
        catch
        {
            // Persist the question even when the model is unreachable, so the user
            // doesn't lose what they typed and the retry has context.
            await db.SaveChangesAsync();
            return StatusCode(StatusCodes.Status502BadGateway,
                new { message = "Asisten sedang tidak tersedia. Silakan coba lagi." });
        }

        db.ChatMessages.Add(new ChatMessage
        {
            UserId = UserId,
            Role = "assistant",
            Content = reply,
            // Nudged past the question so ordering by time never puts the answer first.
            SentAt = DateTime.UtcNow > now ? DateTime.UtcNow : now.AddMilliseconds(1),
        });

        await db.SaveChangesAsync();

        return Ok(new ChatResponseDto(reply));
    }

    /// <summary>Lets the user clear their own conversation. Logout must not do this.</summary>
    [HttpDelete("history")]
    public async Task<IActionResult> ClearHistory()
    {
        await db.ChatMessages.Where(m => m.UserId == UserId).ExecuteDeleteAsync();
        return NoContent();
    }
}
