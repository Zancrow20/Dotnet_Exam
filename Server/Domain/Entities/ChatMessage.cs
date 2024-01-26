namespace Domain.Entities;

public class ChatMessage
{
    public int Id { get; set; }
    public string? From { get; set; }
    public string? To { get; set; }
    public string GameId { get; set; }
    public string Message { get; set; }

    public bool IsSystemMessage => From == "Server" && To == null;
}