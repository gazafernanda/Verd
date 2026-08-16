namespace Verd.Api.Models;

/// <summary>
/// One turn of the assistant conversation, stored per account so the history
/// survives logout and follows the user to another device.
/// </summary>
public class ChatMessage
{
    public int Id { get; set; }
    public int UserId { get; set; }

    /// <summary>user | assistant — matches the role names the chat model expects.</summary>
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
