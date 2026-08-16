namespace Verd.Api.DTOs.Chat;

/// <summary>A persisted conversation turn as returned to the client.</summary>
public record StoredChatMessageDto(
    int Id,
    string Role,
    string Content,
    DateTime SentAt
);
