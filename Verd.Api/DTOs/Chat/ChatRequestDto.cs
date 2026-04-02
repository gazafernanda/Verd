namespace Verd.Api.DTOs.Chat;

public record ChatRequestDto(string Message, List<ChatMessageDto>? History = null);
