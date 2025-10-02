namespace RawJsonImplementation.Models;

public class ChatRequest
{
    public string? Model { get; set; }
    public required List<ChatMessage> Messages { get; set; }
}