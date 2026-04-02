using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Verd.Api.DTOs.Chat;
using Verd.Api.Services;

namespace Verd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController(ChatService chat) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ChatResponseDto>> Send(ChatRequestDto dto)
    {
        var reply = await chat.SendAsync(dto.Message, dto.History);
        return Ok(new ChatResponseDto(reply));
    }
}
